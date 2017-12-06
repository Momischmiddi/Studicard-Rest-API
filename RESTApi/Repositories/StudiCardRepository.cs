using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NFCDataRESTApi.Models;
using NFCDataRESTApi.SQLiteDataBase;
using RESTApi.Exceptions;
using RESTApi.Models.NfcDataModels;

namespace NFCDataRESTApi.Repositories
{
    public class StudiCardRepository
    {
        private DataBase dataBase;

        public StudiCardRepository(DataBase dataBase)
        {
            this.dataBase = dataBase;
        }

        public async Task<string> PutCardInformationAsync(CardInformationModelWithPassword model)
        {
            var info = new CardInfoForStudent();

            info.CardValue = model.CardValue;
            info.LastTransaction = model.LastTransaction;
            info.ID = model.ID;
            info.ScanDate = DateTime.Now.ToString();

            if (!await this.CardHasAlreadyBeenScannedWithThisInformations(info))
            {
                await this.dataBase.CardInformationForStudent.AddAsync(info);
            }
            await this.dataBase.SaveChangesAsync();
            return "Successfully added the card information.";
        }

        private async Task<bool> CardHasAlreadyBeenScannedWithThisInformations(CardInfoForStudent info)
        {
            CardInfoForStudent latestScan = null;

            // Get the latest scan and check, if the data is the same as the scan which has been transmitted
            // If it is so, it is a duplicate, since the student checked his card value twice without adding or removing money.
            var allScansForStudent = await this.dataBase.CardInformationForStudent.Where(scan => scan.ID == info.ID)
                .ToListAsync();

            // If there is no entrance yet, its the students first scan.
            if (!allScansForStudent.Any())
            {
                return false;
            }
            latestScan = allScansForStudent[0];
            foreach (var scan in allScansForStudent)
            {
                var scannedTime = DateTime.Parse(scan.ScanDate);
                if (scannedTime > DateTime.Parse(latestScan.ScanDate))
                {
                    latestScan = scan;
                }
            }

            return latestScan.CardValue == info.CardValue
                   && latestScan.LastTransaction == info.LastTransaction;
        }

        public async Task<CardInformationModelForID> GetCardInformationById(string id)
        {
            var result = new CardInformationModelForID();

            var allCardInformations = await this.dataBase.CardInformationForStudent.Where(cardInfo => cardInfo.ID == id).ToListAsync();
            if (!allCardInformations.Any())
            {
                throw new IDDoesNotExistException($"There is no database-entry with the ID '{id}'.");
            }

            var allValues = new List<double>();
            var allLastTranscation = new List<double>();

            foreach (var cardInformation in allCardInformations)
            {
                allValues.Add(cardInformation.CardValue);
                allLastTranscation.Add(cardInformation.LastTransaction);
            }

            result.AllCurrentValues = allValues;
            result.AllLastTransactions = allLastTranscation;
            result.CurrentValueAverage = this.GetAverage(allValues, allValues.Count);
            result.LastTransactionAverage = this.GetAverage(allLastTranscation, allLastTranscation.Count);

            CardInfoForStudent latestScan = allCardInformations[0];
            foreach (var scan in allCardInformations)
            {
                var scannedTime = DateTime.Parse(scan.ScanDate);
                if (scannedTime > DateTime.Parse(latestScan.ScanDate))
                {
                    latestScan = scan;
                }
            }

            result.CurrentValue = latestScan.CardValue;
            result.LastTransaction = latestScan.LastTransaction;

            var cardStatistics = await this.GetCardStatisticsAsync(id);

            var scanDates = new List<string>();
            var spentMoney = new List<double>();
            var averagesLastTransactions = new List<double>();
            var totalScanAmount = new List<int>();
            foreach (var statistic in cardStatistics)
            {
                var date = statistic.Date.Replace(" 00:00:00", string.Empty).Remove(0, 3).Replace(".", " - ");
                scanDates.Add("Monat: " + date);
                spentMoney.Add(statistic.SpentMoney);
                averagesLastTransactions.Add(statistic.AverageLastTransaction);
                totalScanAmount.Add(statistic.ScanAmount);
            }

            result.ScanDates = scanDates;
            result.SpentMoney = spentMoney;
            result.AverageLastTransactions = averagesLastTransactions;
            result.ScanAmounts = totalScanAmount;

            return result;
        }

        private double GetAverage(List<double> allValues, int amount)
        {
            double result = 0;
            double total = allValues.Sum();
            double average = total / amount;

            return Math.Round(average, 2);
        }

        public async Task<int> GetAllScannedCardsAsync()
        {
            var allIds = new List<string>();

            var allScannedCards = await this.dataBase.CardInformationForStudent.ToListAsync();
            foreach (var scannedCard in allScannedCards)
            {
                if (!allIds.Contains(scannedCard.ID))
                {
                    allIds.Add(scannedCard.ID);
                }
            }

            return allIds.Count;
        }

        public async Task<CardInformationStatisticModel> GetAverageAsync()
        {
            var result = new CardInformationStatisticModel();
            var allCardValues = new List<double>();
            var allLastTranscation = new List<double>();

            var allScannedCards = await this.dataBase.CardInformationForStudent.ToListAsync();
            foreach (var scannedCard in allScannedCards)
            {
                allLastTranscation.Add(scannedCard.LastTransaction);
                allCardValues.Add(scannedCard.CardValue);
            }

            var latestScans = await this.GetLatestScanForEachCard(allScannedCards);

            result.AverageCardValue = this.GetAverage(allCardValues, allCardValues.Count);
            result.AverageLastTransaction = this.GetAverage(allLastTranscation, allLastTranscation.Count);
            result.TotalCardValue = latestScans.Sum(scan => scan.CardValue);
            result.TotalLastTransaction = latestScans.Sum(scan => scan.LastTransaction);

            return result;
        }

        private async Task<List<CardInformationModel>> GetLatestScanForEachCard(List<CardInfoForStudent> allScannedCards)
        {
            var result = new List<CardInformationModel>();
            var allIds = new List<string>();

            foreach (var scan in allScannedCards)
            {
                if (!allIds.Contains(scan.ID))
                {
                    allIds.Add(scan.ID);
                }
            }

            foreach (var id in allIds)
            {
                CardInfoForStudent latestCardInfoForId = new CardInfoForStudent();
                latestCardInfoForId.ScanDate = DateTime.MinValue.ToString();

                var scansWithId = this.dataBase.CardInformationForStudent.Where(card => card.ID == id);
                foreach (var scan in scansWithId)
                {
                    var scanDate = DateTime.Parse(scan.ScanDate);
                    if (scanDate > DateTime.Parse(latestCardInfoForId.ScanDate))
                    {
                        latestCardInfoForId = scan;
                    }
                }
                result.Add(new CardInformationModel
                {
                    ID = latestCardInfoForId.ID,
                    CardValue = latestCardInfoForId.CardValue,
                    LastTransaction = latestCardInfoForId.LastTransaction
                });
            }

            return result;
        }

        public async Task<int> GetTotalScanAmount()
        {
            return await this.dataBase.CardInformationForStudent.CountAsync();
        }

        public async Task<List<AdvancedCardInformationStatistics>> GetCardStatisticsAsync(string id)
        {
            var result = new List<AdvancedCardInformationStatistics>();

            var cardScansOfStudent = await this.dataBase.CardInformationForStudent.Where(card => card.ID == id).ToListAsync();
            if (!cardScansOfStudent.Any())
            {
                throw new IDDoesNotExistException($"There is no database-entry with the ID '{id}'.");
            }
            foreach (var scan in cardScansOfStudent)
            {
                var scanDate = DateTime.Parse(scan.ScanDate);
                var scanDateMonth = new DateTime(scanDate.Year, scanDate.Month, 1);

                var scansInMonth = cardScansOfStudent.Where(card =>
                    scanDate.Year == scanDateMonth.Year && scanDate.Month == scanDateMonth.Month);

                var alreadyExistingStatistics = result.FirstOrDefault(stat =>
                    DateTime.Parse(stat.Date).Year == scanDateMonth.Year && DateTime.Parse(stat.Date).Month == scanDateMonth.Month);

                if (alreadyExistingStatistics == null)
                {
                    var allLastTransaction = new List<double>();
                    foreach (var scanInMonth in scansInMonth)
                    {
                        allLastTransaction.Add(scanInMonth.LastTransaction);
                    }

                    result.Add(new AdvancedCardInformationStatistics
                    {
                        Date = scanDateMonth.ToString(),
                        ScanAmount = scansInMonth.Count(),
                        SpentMoney = scansInMonth.Sum(scans => scans.LastTransaction),
                        AverageLastTransaction = this.GetAverage(allLastTransaction, allLastTransaction.Count)
                    });
                }       
            }

            return result;
        }
    }
}

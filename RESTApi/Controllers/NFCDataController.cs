using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NFCDataRESTApi.Models;
using NFCDataRESTApi.Repositories;
using RESTApi.Models.NfcDataModels;
using RESTApi.Repositories;

namespace NFCDataRESTApi.Controllers
{
    [Route("api/[controller]")]
    public class NFCDataController : Controller
    {
        private readonly StudiCardRepository studiCardRepository;
        private ErrorHandlingRepository errorHandlingRepository;

        public NFCDataController(StudiCardRepository studiCardRepository, ErrorHandlingRepository errorHandlingRepository)
        {
            this.studiCardRepository = studiCardRepository;
            this.errorHandlingRepository = errorHandlingRepository;
        }

        /// <summary>
        /// Puts a cardinformation to the database.
        /// </summary>
        /// <returns>A Message, if writing to the database was successful</returns>
        [HttpPut("/putcardinformation")]
        [Produces("text/plain")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> PutCardInformation([FromBody] CardInformationModelWithPassword model)
        {
            try
            {
                if (this.IsValidPassword(model.Password))
                {
                    var result = await this.studiCardRepository.PutCardInformationAsync(model);
                    Console.WriteLine("Put some Card Information.");
                    return this.Ok(result);
                }
                return this.Unauthorized();
            }
            catch (Exception e)
            {
                this.errorHandlingRepository.LogAndPrintError(e);
                return this.BadRequest(new ErrorModel { ErrorMessage = "Error while putting cardinformations."});
            }
        }

        /// <summary>
        /// Gets the average for all card properties.
        /// </summary>
        /// <returns>A CardInformationModel, which contains the average of each property.</returns>
        [HttpGet("/getaverage")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CardInformationStatisticModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAverage()
        {
            try
            {
                var result = await this.studiCardRepository.GetAverageAsync();
                return this.Ok(result);
            }
            catch (Exception e)
            {
                this.errorHandlingRepository.LogAndPrintError(e);
                return this.BadRequest(new ErrorModel { ErrorMessage = "Error while getting the average for the cardinformations."});
            }
        }

        /// <summary>
        /// Gets the statistics of a card owner.
        /// </summary>
        [HttpGet("/getstatistics/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<AdvancedCardInformationStatistics>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetStatistics(string id)
        {
            try
            {
                var result = await this.studiCardRepository.GetCardStatisticsAsync(id);
                return this.Ok(result);
            }
            catch (Exception e)
            {
                this.errorHandlingRepository.LogAndPrintError(e);
                return this.BadRequest(new ErrorModel { ErrorMessage = "Error while getting the card statistics." });
            }
        }

        /// <summary>
        /// Gets the amount of cards scanned from the arduino.
        /// </summary>
        /// <returns>The amount of the scanned cards.</returns>
        [HttpGet("/scannedcardamount")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetScannedCardAmount()
        {
            try
            {
                var result = await this.studiCardRepository.GetAllScannedCardsAsync();
                return this.Ok(result);
            }
            catch (Exception e)
            {
                this.errorHandlingRepository.LogAndPrintError(e);
                return this.BadRequest(new ErrorModel { ErrorMessage = "Error while getting all availabld card informations."});
            }
        }

        /// <summary>
        /// Gets the informations of a card by id.
        /// </summary>
        /// <returns>The amount of the scanned cards.</returns>
        [HttpGet("/cardinformationsbyid/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<CardInformationModelForID>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCardInformationsByID(string id)
        {
            try
            {
                var result = await this.studiCardRepository.GetCardInformationById(id);
                return this.Ok(result); 
            }
            catch (Exception e)
            {
                this.errorHandlingRepository.LogAndPrintError(e);
                return this.BadRequest(new ErrorModel { ErrorMessage = "Error while getting all availabld card informations: " + e.Message});
            }
        }

        /// <summary>
        /// Gets the amount of all scans that have been made.
        /// </summary>
        /// <returns>The amount of the scanned cards.</returns>
        [HttpGet("/gettotalscanamount")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTotalScanAmount()
        {
            try
            {
                var result = await this.studiCardRepository.GetTotalScanAmount();
                return this.Ok(result);
            }
            catch (Exception e)
            {
                this.errorHandlingRepository.LogAndPrintError(e);
                return this.BadRequest(new ErrorModel { ErrorMessage = "Error while getting all availabld card informations: " + e.Message });
            }
        }

        private bool IsValidPassword(string modelPassword)
        {
            return modelPassword == "9amfdn378fbnsd9s9nn2829mcmh888y8v87v6vb4b32ffw8w9f9fhnnUASDnD932nNGANWMV7anfminOFNAUASDN923734bJKASBUDAXKCNajbadsQUnkasd92734";
        }
    }
}
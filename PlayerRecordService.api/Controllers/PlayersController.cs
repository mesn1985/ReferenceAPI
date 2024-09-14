using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SkycavePlayerService.api.DTOs;
using SkycavePlayerService.api.DTOs.Extentions;
using SkycavePlayerService.api.Infrastructure.Attributes;
using SkycavePlayerService.Shared.Contracts;
using System.ComponentModel.DataAnnotations;
using SkycavePlayerService.Exceptions;
using SkycavePlayerService.Shared.Models.PlayerRecord;
using SkycavePlayerService.Shared.Models.PlayerRecord.Primitives;

namespace SkycavePlayerService.api.Controllers
{
    /// <summary>
    /// Player service service layer.
    /// @author: Team india
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    public class PlayersController : BaseController<PlayersController>
    {
        private IPlayerRepository _playerRepository;

        public PlayersController(
            IPlayerRepository playerRepository,
            ILogger<PlayersController> logger
        ) : base(logger)
        {
            _playerRepository = playerRepository;
        }

        /// <summary>
        /// Gets a player record based on the player id.
        /// </summary>
        /// <param>The player id for the requested player</param>
        /// <response code="200">Player found</response>
        /// <response code="404">Player not found</response>
        [HttpGet("{playerId?}")]
        [ProducesResponseType(typeof(PlayerRecord),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void),StatusCodes.Status404NotFound)]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> GetPlayerRecordById([MinLength(1)]string playerId)
        {
            logger.LogInformation($"Request recieved for Player with id: {playerId}");

            PlayerRecordModel playerModel 
                = await _playerRepository.GetPlayerRecordBy(
                    new PlayerId(playerId)
                    );

            if (playerModel == null)
            {
                logger.LogInformation($"Player with id: {playerId} not found");
                return NotFound();
            }
            logger.LogInformation($"Player with id: {playerId} found");
            return Ok(playerModel.AsDto());
        }
        /// <summary>
        /// Updates existing player, or adds new player if the is no match on the player id
        /// </summary>
        /// <param name="playerRecord">The values that will replace the values of the player with a correlating id </param>
        /// <returns>Status code 200 if request player record is succesfully updated. R</returns>
        /// <response code="200">Player was updated</response>
        /// <response code="201">Player was created</response>
        /// <response code="400">Request contains invalid values for player attributes</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PlayerRecord),StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(void),StatusCodes.Status400BadRequest)]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> UpdatePlayerRecord(PlayerRecord playerRecord)
        {
            PlayerRecordModel playerRecordModel
                = playerRecord.AsModel();

            try
            {
                await _playerRepository.AddPlayer(playerRecordModel);
                logger.LogInformation($"created Player with id: {playerRecordModel.PlayerId.Value}");
                return CreatedAtAction(
                    nameof(GetPlayerRecordById),
                    new { playerId = playerRecordModel.PlayerId.Value},
                    null
                    );
            }
            catch (PlayerExistException exception)
            {
                await _playerRepository.UpdatePlayer(playerRecordModel);
                logger.LogInformation($"updated Player with id: {playerRecordModel.PlayerId.Value}");
                return Ok();
            }

        }
        /// <summary>
        /// Retrieve a list of active players at a specific location within the cave
        /// </summary>
        /// <param name="position">Accepts both Url Encoded and regular strings E.g. (1,2,3) and %281%2C2%2C3%29</param>
        /// <response code="200">One or more active players found at the position</response>
        /// <response code="204">No active players at the position</response>
        /// <response code="400">Position string in wrong format</response>
        [HttpGet("computerListOfPlayersAt")]
        [ProducesResponseType(typeof(List<PlayerRecord>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(void),StatusCodes.Status400BadRequest)]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> ComputeListOfPlayersAt(
                [FromQuery]
                [Position(ErrorMessage = "Position must be in the format of (d,d,d) E.g. (0,2,4)")] string position)
        {
            logger.LogInformation($"Request for Player {position} recieved");
            var listOfPlayerModels 
                = await _playerRepository.GetActivePlayersAt(new Position(position));

            if (listOfPlayerModels?.Count() == 0)
            {
                logger.LogInformation($"No Players found at: {position}");
                return NoContent();
            }

            var playerRecordDtos 
                = listOfPlayerModels.Select(playerRecord => playerRecord.AsDto());

            logger.LogInformation($"Players found at: {position}");
            return Ok(playerRecordDtos);
        }

    }
}

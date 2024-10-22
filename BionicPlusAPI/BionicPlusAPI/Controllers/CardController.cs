using BionicPlusAPI.Data.Concrete;
using BionicPlusAPI.Data.Interfaces;
using BionicPlusAPI.Models;
using DomainObjects.Pregnancy;
using DomainObjects.Pregnancy.Localizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BionicPlusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : Controller
    {
        private readonly ICardRepository _cardRepository;
        private readonly IAnswerRepository _answerRepository;
        private const string BASIC_CARD_ID = "649f06a7c43046eca32bd734";
        public CardController(ICardRepository cardRepository, IAnswerRepository answerRepository)
        {
            _cardRepository = cardRepository;
            _answerRepository = answerRepository;
        }

        [HttpGet("{id}", Name = "GetCardById")]
        [Authorize(Roles = "ADMIN,USER")]
        public async Task<ActionResult<Card>> GetCard(string id, [FromQuery]LocalizationsLanguage language, bool withAnnotations = false)
        {
            var card = await _cardRepository.GetCardByIdAsync(id, language, withAnnotations);
            return Ok(card);
        }

        [HttpGet("all", Name = "GetCardAnnotations")]
        [Authorize(Roles = "ADMIN,USER")]
        public async Task<ActionResult<AnnotationResponse>> GetCards([FromQuery] string? title, [FromQuery] CardType? cardType, [FromQuery] Category? cardCategory, [FromQuery] LocalizationsLanguage language, bool withAnnotations = false)
        {
            var cards = await _cardRepository.GetCards(title, cardType, cardCategory, language, withAnnotations);
            return Ok(cards);
        }

        [HttpGet("", Name = "GetCardByName")]
        [Authorize(Roles = "ADMIN,USER")]
        public async Task<ActionResult> GetCardByName([FromQuery] string title, [FromQuery] LocalizationsLanguage language, bool withAnnotations = false)
        {
            var card = await _cardRepository.GetCardByTitleAsync(title, language, withAnnotations);
            return Ok();
        }

        [HttpGet("basic", Name = "GetBasicCards")]
        [Authorize(Roles = "ADMIN,USER")]
        public async Task<ActionResult<List<Card>>> GetBasicCard(Category category, LocalizationsLanguage language, bool withAnnotations = false)
        {
            var cards = await _cardRepository.GetBasicCardsByCategoryAsync(category, language, withAnnotations);
            return Ok(cards);
        }

        [HttpPost(Name = "AddCard")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<Card>> AddCard(Card card, [FromQuery, Required] LocalizationsLanguage language)
        {
            var addedCard = await _cardRepository.AddCardAsync(card, language);
            return Ok(addedCard);
        }

        [HttpPut("", Name = "UpdateCard")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> UpdateCard([FromBody, Required] Card card, [FromQuery, Required] LocalizationsLanguage language)
        {
            var updateResult = await _cardRepository.UpdateCard(card, language);

            return updateResult ? Ok() : BadRequest();
        }

        [HttpDelete(Name = "DeleteCard")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<Card>> DeleteCard(string id)
        {
            var deletedCard = await _cardRepository.DeleteCardByIdAsync(id);
            return Ok(deletedCard);
        }

        [HttpGet("{cardId}/answer", Name = "GetAnswersByCardId")]
        [Authorize(Roles = "ADMIN,USER")]
        public async Task<ActionResult<List<Answer>>> GetAnswers(string cardId, [FromQuery] LocalizationsLanguage language)
        {
            var answers = await _answerRepository.GetAnswerByCardId(cardId, language);
            return answers;
        }

        [HttpPost("{cardId}/answer", Name = "AddAnswer")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<Answer>> AddAnswer(string cardId, Answer answer, [FromQuery, Required] LocalizationsLanguage language)
        {
            var addResult = await _answerRepository.AddAnswer(cardId, answer, language);

            return addResult != null ? Ok(addResult) : BadRequest();
        }

        [HttpDelete("{cardId}/answer/{answerId}", Name = "DeleteAnswer")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> DeleteAnswer(string cardId, Guid answerId)
        {
            var deleteResult = await _answerRepository.DeleteAnswer(cardId, answerId);

            return deleteResult ? Ok() : BadRequest();
        }

        [HttpPatch("{cardId}/answer/{answerId}", Name = "AssociateAnswer")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> AssociateAnswer(string cardId, Guid answerId, [FromForm, Required] string relatedCardId)
        {
            var updateResult = await _answerRepository.AssociateAnswer(cardId, relatedCardId, answerId);

            return updateResult ? Ok() : BadRequest();
        }

        [HttpPut("{cardId}/answer", Name = "UpdateAnswer")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult> UpdateAnswer(string cardId, [FromBody, Required] Answer answer, [FromQuery, Required] LocalizationsLanguage language)
        {
            var updateResult = await _answerRepository.UpdateAnswer(cardId, answer, language);

            return updateResult ? Ok() : BadRequest();
        }
    }
}

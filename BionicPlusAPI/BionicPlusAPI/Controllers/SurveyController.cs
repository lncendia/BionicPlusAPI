using DomainObjects.Pregnancy.Localizations;
using DomainObjects.Pregnancy;
using Microsoft.AspNetCore.Mvc;
using BionicPlusAPI.Data.Interfaces;
using BionicPlusAPI.Data.Concrete;
using Microsoft.AspNetCore.Authorization;

namespace BionicPlusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SurveyController : Controller
    {
        private readonly ISurveyRepository _surveyRepository;

        public SurveyController(ISurveyRepository surveyRepository, IHttpContextAccessor httpContextAccessor)
        {
            _surveyRepository = surveyRepository;
        }

        [HttpGet("{id}", Name = "GetSurveyById")]
        public async Task<ActionResult<ExtendedSurvey>> GetSurvey(string id, [FromQuery] LocalizationsLanguage language)
        {
            
            var survey = await _surveyRepository.GetSurveyByIdAsync(id, language);
            return Ok(survey);
        }

        [HttpDelete("{id}", Name = "DeleteSurveyById")]
        public async Task<ActionResult<ExtendedSurvey>> DeleteSurvey(string id)
        {
            await _surveyRepository.DeleteSurveyByIdAsync(id);
            return Ok();
        }

        [HttpGet("rollback/{id}", Name = "Rollback")]
        public async Task<ActionResult<bool>> Rollback(string id, [FromQuery] string cardId)
        {
            var survey = await _surveyRepository.Rollback(id, cardId);
            return Ok(survey);
        }

        [HttpPost("answer/{surveyId?}", Name = "Answer")]
        public async Task<ActionResult<Survey>> PostAnswer(SurveyAnswer answer, [FromQuery] LocalizationsLanguage language, string? surveyId)
        {
            var (surId, nextCard, isLastCard) = await _surveyRepository.PostAnswer(surveyId!, answer, language);
            return Ok(new { surveyId = surId, nextCard = nextCard, isLastCard = isLastCard});
        }

        [HttpGet("my/{tag}", Name = "GetMySurveys")]
        public async Task<ActionResult<SurveyMy>> GetSurvey(SurveyTag tag, [FromQuery] LocalizationsLanguage language)
        {
            var survey = await _surveyRepository.GetSurveysByTagAsync(tag, language);
            return Ok(survey);
        }

        [HttpPost("rate/{id}", Name = "RateSurvey")]
        public async Task<ActionResult> RateSurvey(string id, [FromBody] Rating rating)
        {
            if(rating == null)
            {
                return BadRequest("You must fill in the score field");
            }

            if(rating.Score < 1 || rating.Score > 5)
            {
                return BadRequest("The score should be from 1 to 5");
            }

            await _surveyRepository.RateSurvey(id, rating);

            return Ok();
        }
    }
}

using DomainObjects.Pregnancy;
using System.Linq.Expressions;

namespace BionicPlusAPI.Helpers
{
    public static class ConditionHelper
    {
        /// <summary>
        /// This method resolves answer by condition.
        /// For example if condition looks like: "if(int.Parse({0}) > 10) return {1}; else return {2}"
        /// where {0} - userInput, {1} - answer1, {2} - answer2
        /// That transfers to "if(int.Parse(userInput) > 10) return answer1; else return answer2"
        /// </summary>
        /// <param name="card"></param>
        /// <param name="surveyAnswer"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static string ResolveCardId(Card card, SurveyAnswer surveyAnswer)
        {
            if(card.Condition == null || card.Condition.Expressions == null || !card.Condition.Expressions.Any())
            {
                throw new FormatException($"Can't resolve answer, cause the card with id {card.Id} has no expressions");
            }

            if (string.IsNullOrWhiteSpace(surveyAnswer.UserInputValue))
            {
                throw new FormatException($"Can't resolve answer, cause UserInputValue empty");
            }

            var condition = card.Condition;


            var userInput = surveyAnswer.UserInputValue;


            try
            {
                foreach(var expression in condition.Expressions) 
                {
                    var cardId = Evaluate(expression, userInput);
                    if(!string.IsNullOrEmpty(cardId))
                    {
                        return cardId;
                    }
                }
                return condition.DefaultCard!.Id!;
            }
            catch( Exception ex )
            {
                throw new FormatException($"Can't resolve answer, cause expression is composed incorrectly. Reason {ex.Message}");
            }
        }

        private static string Evaluate(DomainObjects.Pregnancy.Expression expression, string userInput)
        {
            var result = Resolve(expression.LeftParameter, expression.RightParameter, expression.OperationType, userInput);

            return result ? expression.Card!.Id! : string.Empty;
        }

        private static bool Resolve(Parameter leftParameter, Parameter rightParameter, OperationType operationType, string userInput)
        {
            if(operationType == OperationType.and || operationType == OperationType.or)
            {
                if(!(leftParameter.ParameterType == ParameterType.expression && leftParameter.ParameterType == ParameterType.expression))
                {
                    throw new FormatException($"Expression incorrect cause Operation type {operationType} work only with expression operand");
                }
            }

            bool left = false;
            bool right = false;
            
            if (leftParameter.ParameterType == ParameterType.expression)
            {
                var expression = leftParameter.Expression;
                left = Resolve(expression.LeftParameter, expression.RightParameter, expression.OperationType, userInput);
            }

            if (rightParameter.ParameterType == ParameterType.expression)
            {
                var expression = rightParameter.Expression;
                right = Resolve(expression.LeftParameter, expression.RightParameter, expression.OperationType, userInput);
            }

            if(leftParameter.ParameterType == ParameterType.value)
            {
                leftParameter.Value = userInput;
            }

            if (rightParameter.ParameterType == ParameterType.value)
            {
                rightParameter.Value = userInput;
            }

            switch (operationType)
            {
                case OperationType.gt:
                    return Double.Parse(leftParameter.Value.ToString()!) > Double.Parse(rightParameter.Value.ToString()!);
                case OperationType.ge:
                    return Double.Parse(leftParameter.Value.ToString()!) >= Double.Parse(rightParameter.Value.ToString()!);
                case OperationType.lt:
                    return Double.Parse(leftParameter.Value.ToString()!) < Double.Parse(rightParameter.Value.ToString()!);
                case OperationType.le:
                    return Double.Parse(leftParameter.Value.ToString()!) <= Double.Parse(rightParameter.Value.ToString()!);
                case OperationType.eq:
                    return Double.Parse(leftParameter.Value.ToString()!) == Double.Parse(rightParameter.Value.ToString()!);
                case OperationType.ne:
                    return Double.Parse(leftParameter.Value.ToString()!) != Double.Parse(rightParameter.Value.ToString()!);
                case OperationType.and:
                    return left && right;
                case OperationType.or:
                    return left || right;
                default:
                    throw new FormatException($"Can't resolve expression");
            }

            throw new FormatException($"Can't resolve expression");
        }
    }
}

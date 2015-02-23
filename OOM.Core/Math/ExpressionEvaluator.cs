using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCalc;

namespace OOM.Core.Math
{
    public class ExpressionEvaluator
    {
        #region Properties

        private Dictionary<string, Func<object[], object>> _evaluateFunctions;

        #endregion

        #region Ctor

        public ExpressionEvaluator()
        {
            _evaluateFunctions = new Dictionary<string, Func<object[], object>>();
            _evaluateFunctions.Add("sum", EvaluateFunctionSum);
            _evaluateFunctions.Add("avg", EvaluateFunctionAvg);
            _evaluateFunctions.Add("max", EvaluateFunctionMax);
            _evaluateFunctions.Add("min", EvaluateFunctionMin);
        }

        #endregion

        #region Public

        public decimal Evaluate(string expression, IDictionary<string, object> parameters = null)
        {
            try
            {
                var e = new Expression(expression, EvaluateOptions.IgnoreCase);

                if (parameters != null)
                    e.Parameters = new Dictionary<string, object>(parameters);

                e.EvaluateFunction += EvaluateFunction;
                return Convert.ToDecimal(e.Evaluate());
            }
            catch (Exception ex)
            {
                throw new ExpressionEvaluationException("The expression evaluation wasn't successful. Maybe the expression has something wrong...", ex);
            }
        }

        #endregion

        #region Privates

        private void EvaluateFunction(string name, FunctionArgs args)
        {
            var key = name.ToLowerInvariant();
            if (!_evaluateFunctions.ContainsKey(key))
                return;

            try
            {
                args.Result = _evaluateFunctions[key].Invoke(args.EvaluateParameters());
            }
            catch (Exception) { }
        }

        private object EvaluateFunctionSum(object[] parameters)
        {
            if (parameters.Length <= 0)
                throw new Exception();

            object result = 0;
            if (parameters.Length > 1)
            {
                foreach (var parameter in parameters)
                    result = Numbers.Add(result, parameter);
            }
            else
            {
                if (!parameters[0].GetType().IsArray)
                    throw new Exception();

                foreach (var parameter in (Array)parameters[0])
                    result = Numbers.Add(result, parameter);
            }

            return result;
        }

        private object EvaluateFunctionAvg(object[] parameters)
        {
            if (parameters.Length <= 0)
                throw new Exception();

            object result = 0;
            object totalCount = 0;
            if (parameters.Length > 1)
            {
                totalCount = parameters.Length;
                foreach (var parameter in parameters)
                    result = Numbers.Add(result, parameter);
            }
            else
            {
                if (!parameters[0].GetType().IsArray)
                    throw new Exception();

                var a = (Array)parameters[0];
                totalCount = a.Length;
                foreach (var parameter in a)
                    result = Numbers.Add(result, parameter);
            }

            return Numbers.Divide(result, totalCount);
        }

        private object EvaluateFunctionMax(object[] parameters)
        {
            if (parameters.Length <= 0)
                throw new Exception();

            object result = Decimal.MinValue;
            if (parameters.Length > 1)
            {
                foreach (var parameter in parameters)
                    result = Numbers.Max(result, parameter);
            }
            else
            {
                if (!parameters[0].GetType().IsArray)
                    throw new Exception();

                foreach (var parameter in (Array)parameters[0])
                    result = Numbers.Max(result, parameter);
            }

            return result;
        }

        private object EvaluateFunctionMin(object[] parameters)
        {
            if (parameters.Length <= 0)
                throw new Exception();

            object result = Decimal.MaxValue;
            if (parameters.Length > 1)
            {
                foreach (var parameter in parameters)
                    result = Numbers.Min(result, parameter);
            }
            else
            {
                if (!parameters[0].GetType().IsArray)
                    throw new Exception();

                foreach (var parameter in (Array)parameters[0])
                    result = Numbers.Min(result, parameter);
            }

            return result;
        }

        #endregion
    }

    public class ExpressionEvaluationException : Exception {
        public ExpressionEvaluationException(string message, Exception innerException)
            : base(message, innerException) { }

        public ExpressionEvaluationException(string message)
            : base(message) { }

        public ExpressionEvaluationException()
            : base() { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCalc;

namespace OOM.Core.Math
{
    public sealed class ExpressionEvaluator
    {
        #region Singleton

        static readonly ExpressionEvaluator _instance = new ExpressionEvaluator();
        public static ExpressionEvaluator Instance { get { return _instance; } }
        static ExpressionEvaluator() { }

        #endregion

        #region Properties

        private Dictionary<string, Func<object[], object>> _evaluateFunctions;

        #endregion

        #region Ctor

        ExpressionEvaluator()
        {
            _evaluateFunctions = new Dictionary<string, Func<object[], object>>();
            _evaluateFunctions.Add("tanh", EvaluateFunctionTanh);
            _evaluateFunctions.Add("cosh", EvaluateFunctionCosh);
            _evaluateFunctions.Add("sinh", EvaluateFunctionSinh);
            _evaluateFunctions.Add("csc", EvaluateFunctionCsc);
            _evaluateFunctions.Add("sec", EvaluateFunctionSec);
            _evaluateFunctions.Add("cot", EvaluateFunctionCot);
            _evaluateFunctions.Add("mod", EvaluateFunctionMod);
            _evaluateFunctions.Add("lcm", EvaluateFunctionLcm);
            _evaluateFunctions.Add("gcd", EvaluateFunctionGcd);
            _evaluateFunctions.Add("log", EvaluateFunctionLog);
            _evaluateFunctions.Add("ln", EvaluateFunctionLn);
            _evaluateFunctions.Add("sum", EvaluateFunctionSum);
            _evaluateFunctions.Add("avg", EvaluateFunctionAvg);
            _evaluateFunctions.Add("max", EvaluateFunctionMax);
            _evaluateFunctions.Add("min", EvaluateFunctionMin);
        }

        #endregion

        #region Public

        public decimal Evaluate(string expression, IDictionary<string, object> parameters = null)
        {
            var e = new Expression(expression, EvaluateOptions.IgnoreCase);
            e.EvaluateFunction += EvaluateFunction;

            if (parameters != null)
                e.Parameters = new Dictionary<string, object>(parameters);

            try
            {
                return Convert.ToDecimal(e.Evaluate());
            }
            catch (Exception ex)
            {
                throw new ExpressionEvaluationException(String.Format("The expression evaluation wasn't successful. Maybe the expression has something wrong... it says: \"{0}\"", e.Error), ex);
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
                var parameters = args.EvaluateParameters();
                args.Result = _evaluateFunctions[key].Invoke(SanitizeParameters(ref parameters));
            }
            catch (Exception) { }
        }

        private object EvaluateFunctionLn(object[] parameters)
        {
            double value;
            if (!Double.TryParse(parameters[0].ToString(), out value))
                throw new Exception();

            return Convert.ToDecimal(System.Math.Log(value));
        }

        private object EvaluateFunctionLog(object[] parameters)
        {
            double value;
            if (!Double.TryParse(parameters[0].ToString(), out value))
                throw new Exception();

            return Convert.ToDecimal(System.Math.Log10(value));
        }

        private object EvaluateFunctionGcd(object[] parameters)
        {
            long a, b;
            if (!Int64.TryParse(parameters[0].ToString(), out a) || !Int64.TryParse(parameters[1].ToString(), out b))
                throw new Exception();

            return GCD(a, b);
        }

        private object EvaluateFunctionLcm(object[] parameters)
        {
            long a, b;
            if (!Int64.TryParse(parameters[0].ToString(), out a) || !Int64.TryParse(parameters[1].ToString(), out b))
                throw new Exception();

            return LCM(a, b);
        }

        private object EvaluateFunctionMod(object[] parameters)
        {
            double value;
            if (!Double.TryParse(parameters[0].ToString(), out value))
                throw new Exception();

            return value > 0 ? value : -value;
        }

        private object EvaluateFunctionCot(object[] parameters)
        {
            double value;
            if (!Double.TryParse(parameters[0].ToString(), out value))
                throw new Exception();

            return Convert.ToDecimal(1 / System.Math.Tan(value));
        }

        private object EvaluateFunctionSec(object[] parameters)
        {
            double value;
            if (!Double.TryParse(parameters[0].ToString(), out value))
                throw new Exception();

            return Convert.ToDecimal(1 / System.Math.Cos(value));
        }

        private object EvaluateFunctionCsc(object[] parameters)
        {
            double value;
            if (!Double.TryParse(parameters[0].ToString(), out value))
                throw new Exception();

            return Convert.ToDecimal(1 / System.Math.Sin(value));
        }

        private object EvaluateFunctionSinh(object[] parameters)
        {
            double value;
            if (!Double.TryParse(parameters[0].ToString(), out value))
                throw new Exception();

            return Convert.ToDecimal(System.Math.Sinh(value));
        }

        private object EvaluateFunctionCosh(object[] parameters)
        {
            double value;
            if (!Double.TryParse(parameters[0].ToString(), out value))
                throw new Exception();

            return Convert.ToDecimal(System.Math.Cosh(value));
        }

        private object EvaluateFunctionTanh(object[] parameters)
        {
            double value;
            if (!Double.TryParse(parameters[0].ToString(), out value))
                throw new Exception();

            return Convert.ToDecimal(System.Math.Tanh(value));
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
                    return parameters[0];

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
                    return parameters[0];

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
                    return parameters[0];

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
                    return parameters[0];

                foreach (var parameter in (Array)parameters[0])
                    result = Numbers.Min(result, parameter);
            }

            return result;
        }

        /// <summary>
        /// Find the Greatest Common Divisor
        /// </summary>
        /// <param name="a">Number a</param>
        /// <param name="b">Number b</param>
        /// <returns>The greatest common Divisor</returns>
        private long GCD(long a, long b)
        {
            while (b != 0)
            {
                long tmp = b;
                b = a % b;
                a = tmp;
            }

            return a;
        }

        /// <summary>
        /// Find the Least Common Multiple
        /// </summary>
        /// <param name="a">Number a</param>
        /// <param name="b">Number b</param>
        /// <returns>The least common multiple</returns>
        private long LCM(long a, long b)
        {
            return (a * b) / GCD(a, b);
        }

        private object[] SanitizeParameters(ref object[] parameters)
        {
            for (long i = 0; i < parameters.LongLength; i++)
            {
                if (parameters[i] is Double && Double.IsNaN((Double)parameters[i]))
                    parameters[i] = 0d;
            }
            
            return parameters;
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

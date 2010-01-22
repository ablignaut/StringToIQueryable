using System;
using System.Collections.Generic;
using System.Linq;

namespace ParserLib
{
    public static class Parser
    {
        public static Parse<T> Fail<T>()
        {
            return input => null;
        }
        public static Parse<T> Return<T>(T x)
        {
            return input => new ParserResult<T>(x, input);
        }
        public static Parse<U> Then<T, U>(this Parse<T> p1, Func<T, Parse<U>> f)
        {
            return input =>
            {
                ParserResult<T> result1 = p1(input);
                if (result1 == null)
                    return null;
                else
                    return f(result1.Parsed)(result1.Remaining);
            };
        }
        public static Parse<T> Or<T>(this Parse<T> p1, Parse<T> p2)
        {
            return input =>
            {
                ParserResult<T> result1 = p1(input);
                if (result1 == null)
                    return p2(input);
                return result1;
            };
        }

        public static Parse<char> Item()
        {
            return input => string.IsNullOrEmpty(input) ? null : new ParserResult<char>(input[0], input.Substring(1));
        }

        public static Parse<U> Then<T, U>(this Parse<T> p1, Parse<U> p2)
        {
            return p1.Then(_ => p2);
        }

        public static Parse<T> Literal<T>(string toParse, T result)
        {
            if (string.IsNullOrEmpty(toParse))
                return Parser.Return(result);
            return Parser.Item().Where(c => c == toParse[0])
                                .Then(Literal(toParse.Substring(1), result));
        }

        public static Parse<TValue> Where<TValue>(this Parse<TValue> parser, Func<TValue, bool> pred)
        {
            return input =>
            {
                var res = parser(input);
                return res == null || !pred(res.Parsed) ? null : res;
            };
        }

        public static Parse<TValue2> Select<TValue, TValue2>(this Parse<TValue> parser, Func<TValue, TValue2> selector)
        {
            return input =>
            {
                var res = parser(input);
                return res == null ? null : new ParserResult<TValue2>(selector(res.Parsed), res.Remaining);
            };
        }

        public static Parse<TValue2> SelectMany<TValue, TIntermediate, TValue2>(this Parse<TValue> parser,
            Func<TValue, Parse<TIntermediate>> selector, Func<TValue, TIntermediate, TValue2> projector)
        {
            return input =>
            {
                var res = parser(input);
                
                if (res == null) 
                    return null;

                var val = res.Parsed;
                var res2 = selector(val)(res.Remaining);
                
                if (res2 == null) 
                    return null;
                
                return new ParserResult<TValue2>(projector(val, res2.Parsed), res2.Remaining);
            };
        }

        public static Parse<IEnumerable<TValue>> Repeat<TValue>(this Parse<TValue> parser)
        {
            return input =>
            {
                var list = new List<TValue>();
                bool parsedAll = false;

                while (!parsedAll)
                {
                    var parse = parser(input);
                    if (string.IsNullOrEmpty(input) || parse == null)
                        parsedAll = true;
                    else
                    {
                        list.Add(parse.Parsed);
                        input = parse.Remaining;
                    }
                }
                return new ParserResult<IEnumerable<TValue>>(list.AsEnumerable(), string.Empty);
            };
        }

        public static Parse<T> When<T>(Predicate<string> inputPredicate, Parse<T> parser)
        {
            return input =>
            {
                if (inputPredicate(input))
                    return parser(input);
                return null;
            };
        }

        public static Parse<T> OrWhen<T>(this Parse<T> orig, Predicate<string> inputPredicate, Parse<T> parser)
        {
            return input =>
            {
                var origResult = orig(input);

                if (origResult != null)
                    return origResult;
                if (inputPredicate(input))
                    return parser(input);
                return null;
            };
        }
    }
}

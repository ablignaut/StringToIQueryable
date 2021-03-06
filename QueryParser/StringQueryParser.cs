﻿using System;
using System.Collections.Generic;
using System.Linq;
using ParserLib;

namespace QueryParser
{
    public class StringQueryParser<T>
    {
        private string expressions;
        public StringQueryParser(string exprs)
        {
            expressions = exprs.ToLower();
        }

        protected Predicate<string> StartsWith(string test)
        {
            return str => str.StartsWith(test);
        }

        protected virtual Parse<IParserExpression<T>> Parsers
        {
            get
            {
                return Parser.When(StartsWith(ParserConstants.ExpressionSeparator.ToString()), IgnoreSeparatorParser)
                         .OrWhen(StartsWith(ParserConstants.PageIndicator), PageParser)
                         .OrWhen(StartsWith(ParserConstants.SkipIndicator), SkipParser)
                         .OrWhen(StartsWith(ParserConstants.TakeIndicator), TakeParser)
                         .OrWhen(StartsWith(ParserConstants.SortIndicator), SortParser)
                         .Or(WhereParser); 
            }
        }

        public IEnumerable<IParserExpression<T>> Parse()
        {
            if (string.IsNullOrEmpty(expressions))
                return new List<IParserExpression<T>>();

            return Parsers.Repeat()
                          .Invoke(expressions)
                          .Parsed
                          .Where(x => x != null);
        }

        public IQueryable<T> Map(IQueryable<T> queriable)
        {
            foreach (var parseExpr in Parse())
                queriable = parseExpr.Map(queriable);

            return queriable;
        }

        public static Parse<IParserExpression<T>> IgnoreSeparatorParser
        {
            get
            {
                return input => new ParserResult<IParserExpression<T>>(null, input.TrimStart(ParserConstants.ExpressionSeparator));
            }
        }

        public static Parse<IParserExpression<T>> SortParser
        {
            get
            {
                return input =>
                    {
                        string rest;
                        string sortRemoved = input.SeparateAt(ParserConstants.ExpressionSeparator, out rest)
                                                  .Replace(ParserConstants.SortIndicator, "");

                        bool desc = sortRemoved.StartsWith(ParserConstants.DescendingIndicator);

                        if (desc)
                            sortRemoved = sortRemoved.Replace(ParserConstants.DescendingIndicator, "");

                        return new ParserResult<IParserExpression<T>>
                        (
                            new SortExpression<T>(sortRemoved.Split(ParserConstants.ListDeliminator), desc),
                            rest
                        );
                    };
            }
        }

        public static Parse<IParserExpression<T>> SkipParser
        {
            get
            {
                return input =>
                {
                    string rest;
                    string skipNumberToParse = input.SeparateAt(ParserConstants.ExpressionSeparator, out rest)
                                                    .Replace(ParserConstants.SkipIndicator, "");
                    int skipNum = -1;
                    if (!int.TryParse(skipNumberToParse, out skipNum))
                        return null;

                    return new ParserResult<IParserExpression<T>>
                    (
                        new SkipExpression<T>(skipNum),
                        rest
                    );
                };
            }
        }

        public static Parse<IParserExpression<T>> TakeParser
        {
            get
            {
                return input =>
                {
                    string rest;
                    string takeNumberToParse = input.SeparateAt(ParserConstants.ExpressionSeparator, out rest)
                                                    .Replace(ParserConstants.TakeIndicator, "");
                    int takeNum = -1;
                    if (!int.TryParse(takeNumberToParse, out takeNum) || takeNum == -1)
                        return new ParserResult<IParserExpression<T>>
                        (
                            TakeExpression<T>.All(),
                            rest
                        );

                    return new ParserResult<IParserExpression<T>>
                    (
                        new TakeExpression<T>(takeNum),
                        rest
                    );
                };
            }
        }

        public static Parse<IParserExpression<T>> PageParser
        {
            get
            {
                return input =>
                {
                    int pagenum = 1, pagesize = ParserConstants.DefaultPageSize;
                    string rest;
                    string[] pageExpr = input.SeparateAt(ParserConstants.ExpressionSeparator, out rest)
                                             .Replace(ParserConstants.PageIndicator, "")
                                             .Split(ParserConstants.InnerExpressionSeparator);

                    if (pageExpr.Length == 0 || pageExpr.Length > 2)
                        return null;

                    if (pageExpr.Length == 2)
                        int.TryParse(pageExpr[1], out pagesize);

                    int.TryParse(pageExpr[0], out pagenum);

                    return new ParserResult<IParserExpression<T>>
                    (
                        new PageExpression<T>(pagenum, pagesize),
                        rest
                    );
                };
            }
        }

        public static Parse<IParserExpression<T>> WhereParser
        {
            get
            {
                return input =>
                {
                    string rest;
                    string[] split = input.SeparateAt(ParserConstants.ExpressionSeparator, out rest)
                                          .Split(ParserConstants.InnerExpressionSeparator);

                    if (split.Length != 3)
                        return null;

                    WhereCombinator combinator = (WhereCombinator)Enum.Parse(typeof(WhereCombinator), split[1]);
                    return new ParserResult<IParserExpression<T>>
                    (
                        new WhereExpression<T>(split[0], combinator, split[2]),
                        rest
                    );
                };
            }
        }
    }
}

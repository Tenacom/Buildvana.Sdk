// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

namespace Buildvana.Sdk.Tasks.Internal
{
    internal abstract class CodeGenerator<TBuilder>
        where TBuilder : CodeBuilder, new()
    {
        private readonly TBuilder _builder;

        protected CodeGenerator()
        {
            _builder = new TBuilder();
        }

        protected bool AtStartOfLine => _builder.AtStartOfLine;

        protected void Text(char ch) => _builder.Text(ch);

        protected void Text(string code) => _builder.Text(code);

        protected void Text(string code, int startIndex, int count) => _builder.Text(code, startIndex, count);

        protected void QuotedText(string str) => _builder.QuotedText(str);

        protected void NewLine() => _builder.NewLine();

        protected void NewLine(string code) => _builder.NewLine(code);

        protected void LineComment(string comment) => _builder.LineComment(comment);

        protected string GetGeneratedCode() => _builder.GetGeneratedCode();
    }
}
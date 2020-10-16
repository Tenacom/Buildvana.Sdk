// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

using System.Text;

namespace Buildvana.Sdk.Tasks.Internal
{
    internal abstract class CodeBuilder
    {
        private readonly StringBuilder _sb;

        protected CodeBuilder()
        {
            _sb = new StringBuilder();
            AtStartOfLine = true;
        }

        protected bool AtStartOfLine { get; private set; }

        public string GetGeneratedCode() => _sb.ToString();

        protected void Text(char ch)
        {
            _ = _sb.Append(ch);
            AtStartOfLine = false;
        }

        protected void Text(string str)
        {
            _ = _sb.Append(str);
            AtStartOfLine = false;
        }

        protected void Text(string str, int startIndex, int count)
        {
            _ = _sb.Append(str, startIndex, count);
            AtStartOfLine = false;
        }

        protected void NewLine()
        {
            if (!AtStartOfLine)
            {
                _ = _sb.AppendLine();
                AtStartOfLine = true;
            }
        }

        protected void NewLine(string str)
        {
            NewLine();
            Text(str);
        }
    }
}
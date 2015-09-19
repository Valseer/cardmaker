﻿////////////////////////////////////////////////////////////////////////////////
// The MIT License (MIT)
//
// Copyright (c) 2015 Tim Stair
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using CardMaker.XML;

namespace CardMaker.Card.FormattedText
{
    public abstract class MarkupBase
    {
        protected MarkupBase()
        {
            TargetRect = RectangleF.Empty;
        }

        public int LineNumber { get; set; }

        public virtual bool Aligns
        {
            get { return false; }
        }

        public RectangleF TargetRect { get; set; }

        /// <summary>
        /// Processes the markup to determine the markup stack information (font settings, rectangle sizes/settings)
        /// </summary>
        /// <param name="zElement"></param>
        /// <param name="zData"></param>
        /// <param name="zProcessData"></param>
        /// <param name="zGraphics"></param>
        /// <returns>true if this markup is to be further processed</returns>
        public virtual bool ProcessMarkup(ProjectLayoutElement zElement, FormattedTextData zData, FormattedTextProcessData zProcessData, Graphics zGraphics)
        {
            return false;
        }

        /// <summary>
        /// Second pass after rectangles are configured
        /// </summary>
        /// <param name="zElement"></param>
        /// <param name="listAllMarkups"></param>
        /// <param name="nMarkup"></param>
        /// <returns>true if this markup is to be further processed</returns>
        public virtual bool PostProcessMarkupRectangle(ProjectLayoutElement zElement, List<MarkupBase> listAllMarkups, int nMarkup)
        {
            return false;
        }

        public virtual void CloseMarkup(FormattedTextData zData, FormattedTextProcessData zProcessData, Graphics zGraphics) { }

        public virtual bool Render(ProjectLayoutElement zElement, Graphics zGraphics)
        {
            return true;
        }

        private static readonly Dictionary<String, Type> s_dictionaryMarkupTypes = new Dictionary<string, Type>()
        {
            {"b", typeof (FontStyleBoldMarkup)},
            {"i", typeof (FontStyleItalicMarkup)},
            {"s", typeof (FontStyleStrikeoutMarkup)},
            {"u", typeof (FontStyleUnderlineMarkup)},
            {"f", typeof (FontMarkup)},
            {"fs", typeof (FontSizeMarkup)},
            {"fc", typeof (FontColorMarkup)},
            {"xo", typeof (XDrawOffsetMarkup)},
            {"yo", typeof (YDrawOffsetMarkup)},
            {"br", typeof (NewlineMarkup)},
            {"bgc", typeof (BackgroundColorMarkup)},
            {"bgi", typeof (BackgroundImageMarkup)},
            {"spc", typeof (SpaceMarkup)},
            {"push", typeof (PushMarkup)},
            {"img", typeof (ImageMarkup)},
        };

        public static Type GetMarkupType(string sInput)
        {
            Type zType;
            if (s_dictionaryMarkupTypes.TryGetValue(sInput.ToLower(), out zType))
            {
                return zType;
            }
            return null;
        }

        public static MarkupBase GetMarkup(string sInput)
        {
            // check for the value based tags
            Type typeMarkup;
            var arraySplit = sInput.Split(new char[] { '=' });

            try
            {
                if (2 == arraySplit.Length)
                {
                    typeMarkup = GetMarkupType(arraySplit[0]);
                    if (null != typeMarkup)
                    {
                        return (MarkupBase) Activator.CreateInstance(typeMarkup, new object[] {arraySplit[1]});
                    }
                }
                else
                {
                    // this check is after the value based tag check because some tags may optionally use values
                    typeMarkup = GetMarkupType(sInput);
                    if (null != typeMarkup)
                    {
                        return (MarkupBase) Activator.CreateInstance(typeMarkup);
                    }
                }
            }
            catch (Exception)
            {}
            return null;
        }    
    }
}
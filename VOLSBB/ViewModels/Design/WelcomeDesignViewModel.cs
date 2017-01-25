﻿//-----------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Volsbb.Universal.Models;

using Windows.ApplicationModel.Resources;

namespace Volsbb.Universal.ViewModels.Design
{
    /// <summary>
    /// The design-time ViewModel for the welcome view.
    /// </summary>
    public class WelcomeDesignViewModel
    {
       // public static string appName = ResourceLoader.GetForCurrentView().GetString("AppName/Text");
     

        public IList<InstructionItem> InstructionItems = new List<InstructionItem>
            {
                new InstructionItem("Welcome",
                    $" gives you the opportunity to share interesting moments of your life with people who have the same passion.",
                    new Uri("ms-appx:///Assets/Welcome/GiveGold.jpg")),
                new InstructionItem("Ready?",
                    "That was easy, right? Now you should be ready to upload your first photo.",
                    null,
                    "MainPage")
            };

        public InstructionItem SelectedInstructionItem { get; set; }
    }
}
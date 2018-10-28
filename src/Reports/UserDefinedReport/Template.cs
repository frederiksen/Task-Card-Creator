// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;
using ReportingFramework;
using ReportInterface;

namespace UserDefinedReport
{
    [Export(typeof(IReport))]
    public partial class Template : ReportFromMemory, IReport
    {
        public bool IsSupported(IEnumerable<string> wiTypeCollection)
        {
            return true;
        }

        public string Description
        {
            get
            {
                try
                {
                    var fileName = FileName(string.Format("UserDefinedReport.xml"));
                    if (File.Exists(fileName))
                    {
                        var doc = new XmlDocument();
                        doc.Load(fileName);
                        var root = doc.DocumentElement;
                        var description = root.Attributes["description"].Value;
                        if (description != null)
                            return description;
                    }
                }
                catch (Exception exception)
                {
                }

                return "User Defined Report (currently undefined)";
            }
        }

        public Size PaperSize
        {
            get
            {
                try
                {
                    var fileName = FileName(string.Format("UserDefinedReport.xml"));
                    if (File.Exists(fileName))
                    {
                        var doc = new XmlDocument();
                        doc.Load(fileName);
                        var root = doc.DocumentElement;
                        var width = root.Attributes["width"].Value;
                        var height = root.Attributes["height"].Value;
                        if (width != null && height != null)
                        {
                            var numberFormatter = new NumberFormatInfo() { NumberDecimalSeparator = "." };
                            return new Size(Convert.ToDouble(width, numberFormatter), Convert.ToDouble(height, numberFormatter));
                        }
                    }
                }
                catch (Exception exception)
                {
                }

                return new Size(8.27, 11.69);
            }
        }

        public Margins Margins
        {
            get
            {
                try
                {
                    var fileName = FileName(string.Format("UserDefinedReport.xml"));
                    if (File.Exists(fileName))
                    {
                        var doc = new XmlDocument();
                        doc.Load(fileName);
                        var root = doc.DocumentElement;
                        var leftmargin = root.Attributes["leftmargin"].Value;
                        var rightmargin = root.Attributes["rightmargin"].Value;
                        var topmargin = root.Attributes["topmargin"].Value;
                        var bottommargin = root.Attributes["bottommargin"].Value;
                        if (leftmargin != null && rightmargin != null && topmargin != null && bottommargin != null)
                        {
                            return new Margins(
                              Convert.ToInt32(leftmargin),
                              Convert.ToInt32(rightmargin),
                              Convert.ToInt32(topmargin),
                              Convert.ToInt32(bottommargin)
                              );
                        }
                    }
                }
                catch (Exception exception)
                {
                }

                return new Margins(0, 0, 0, 0);
            }
        }

        public bool TeamCustomized { get { return true; } }
        public FixedDocument Create(IEnumerable<ReportItem> data)
        {
            var rows = new List<object>();
            foreach (var workItem in data)
            {
                var fileName = FileName(string.Format("{0}.xaml", workItem.Type));
                try
                {
                    var text = string.Empty;
                    using (var streamReader = new StreamReader(fileName, Encoding.UTF8))
                    {
                        text = streamReader.ReadToEnd();
                    }
                    var xaml = XamlReader.Parse(text) as FrameworkElement;

                    if (xaml != null)
                    {
                        var idTextBlock = xaml.FindName("Id") as TextBlock;
                        if (idTextBlock != null) idTextBlock.Text = string.Format("{0}", workItem.Id).DashIfEmpty();

                        var parentIdTextBlock = xaml.FindName("ParentId") as TextBlock;
                        if (parentIdTextBlock != null) parentIdTextBlock.Text = string.Format("{0}", workItem.ParentId).DashIfEmpty();

                        var titleTextBlock = xaml.FindName("Title") as TextBlock;
                        if (titleTextBlock != null) titleTextBlock.Text = string.Format("{0}", workItem.Title).DashIfEmpty();

                        var typeTextBlock = xaml.FindName("Type") as TextBlock;
                        if (typeTextBlock != null) typeTextBlock.Text = string.Format("{0}", workItem.Type).DashIfEmpty();

                        var stateTextBlock = xaml.FindName("State") as TextBlock;
                        if (stateTextBlock != null) stateTextBlock.Text = string.Format("{0}", workItem.State).DashIfEmpty();

                        var descriptionTextBlock = xaml.FindName("Description") as TextBlock;
                        if (descriptionTextBlock != null) descriptionTextBlock.Text = string.Format("{0}", workItem.Description).DashIfEmpty();

                        foreach (var field in workItem.Fields)
                        {
                            var name = ("Field_" + field.Key).Replace(" ", "_");
                            var fieldTextBlock = xaml.FindName(name) as TextBlock;

                            if (fieldTextBlock != null)
                            {
                                if (fieldTextBlock.Tag != null && fieldTextBlock.Tag is string)
                                {
                                    var controlsFromTag = GetControlsFromTag(fieldTextBlock.Tag, xaml);
                                    controlsFromTag.ForEach(control => FillControlBasedOnTag(control, field.Value));
                                }
                                else
                                {
                                    fieldTextBlock.Text = string.Format("{0}", field.Value).DashIfEmpty();
                                }
                            }
                        }

                        rows.Add(xaml);
                    }
                }
                catch (Exception exception)
                {
                    var textBlock = new TextBlock() { Text = exception.Message, Margin = new Thickness(5), TextWrapping = TextWrapping.Wrap };
                    rows.Add(textBlock);

                    var howtoBlock = new TextBlock() { Text = "How to create a User Defined Report:", Margin = new Thickness(5), TextWrapping = TextWrapping.Wrap };
                    rows.Add(howtoBlock);

                    var guideUrlBlock = new TextBlock() { Text = "https://github.com/frederiksen/Task-Card-Creator", Margin = new Thickness(5), TextWrapping = TextWrapping.Wrap };
                    rows.Add(guideUrlBlock);
                }
            }

            return GenerateReport(
                page => new PageInfo(page, DateTime.Now),
                PaperSize,
                Margins,
                rows
                );
        }

        private static List<TextBlock> GetControlsFromTag(object tag, FrameworkElement xaml)
        {
            var tagData = tag as string;

            if (tagData == null)
            {
                return new List<TextBlock>();
            }

            var controlNames = tagData.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return controlNames.Select(xaml.FindName).Cast<TextBlock>().ToList();
        }

        private static void FillControlBasedOnTag(TextBlock control, object value)
        {
            var tagData = control.Tag as string;
            var fieldValue = value as string;

            if (string.IsNullOrWhiteSpace(tagData) || string.IsNullOrWhiteSpace(fieldValue))
            {
                return;
            }

            var phrasesMapping = tagData.Split(new[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);
            var searchesPhrase = phrasesMapping[0].Trim();
            var targetPhrase = phrasesMapping[1].Trim();

            var isPhrasePresent = fieldValue
                                    .Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                                    .Contains(searchesPhrase);

            if (isPhrasePresent)
            {
                control.Text = targetPhrase;
            }
        }

        private static string FileName(string s)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return string.Format("{0}\\TaskCardCreator\\{1}", path, s);
        }

        /// <summary>
        /// This class becomes the data context for every page. It gives the page 
        /// access to the page number.
        /// </summary>
        private class PageInfo
        {
            public PageInfo(int pageNumber, DateTime reportDate)
            {
                PageNumber = pageNumber;
                ReportDate = reportDate;
            }

            public bool IsFirstPage { get { return PageNumber == 1; } }
            public int PageNumber { get; set; }
            public DateTime ReportDate { get; set; }
        }
    }
}

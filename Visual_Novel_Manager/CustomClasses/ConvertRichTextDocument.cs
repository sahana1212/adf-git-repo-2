using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;

namespace Visual_Novel_Manager.CustomClasses
{
    public class ConvertRichTextDocument
    {
        public FlowDocument ConvertToFlowDocument(string text)
        {
            var flowDocument = new FlowDocument();

            var startBBregex = new Regex(@"(\[url=.+?\])", RegexOptions.Compiled | RegexOptions.IgnoreCase);//matches the first part of bbcode url([url=website]
            var matchStartBBregex = startBBregex.Matches(text).Cast<Match>().Select(m => m.Value).ToList();

            List<string> urlList = new List<string>();



            foreach (var itm in startBBregex.Split(text))
            {
                if (matchStartBBregex.Contains(itm))
                {

                    var vndbRegex = new Regex(@"(\[\burl\b=\/[a-z][0-9]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);//matches the local url, but with the url tag, to eliminate bad values

                    foreach (var url in vndbRegex.Split(itm))
                    {
                        var localRegex = new Regex(@"(\/[a-z][0-9]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);//matches vndb local url, like /c##, /v##,...
                        var localRegexmatch = localRegex.Match(itm).ToString();
                        if (localRegexmatch != "" && !urlList.Contains("http://vndb.org" + localRegexmatch))
                        {
                            urlList.Add("http://vndb.org" + localRegexmatch);
                        }

                    }



                    var regexURL = new Regex(@"(?:\[url=)(.+?)(?:\])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    //above regex matches the url within the bbcode. So everything after url= and NOT including the last bracket
                    var testList = regexURL.Split(itm);

                    foreach (var url in testList)
                    {
                        if (url != "" && !urlList.Contains(url))
                        {
                            //use this for the complete URL matching regex, currently only using part of it: http://blog.mattheworiordan.com/post/13174566389/url-regular-expression-for-links-with-or-without
                            var UrlRegex = new Regex(@"(([A-Za-z]{3,9}:(?:\/\/)?)[A-Za-z0-9\.\-]+|(?:www\.)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-]*)?\??(?:[\-\+=&;%@\.\w]*)#?(?:[\.\!\/\\\w]*)?)", RegexOptions.Compiled | RegexOptions.IgnoreCase);//matches full URLs

                            if (url == UrlRegex.Match(url).ToString())
                            {
                                urlList.Add(url);
                            }

                        }
                    }

                }
            }


            var fullBBcodeRegex = new Regex(@"(\[url=.+?\].+?\[\/url\])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var centerwordBbRegex = new Regex(@"(?:\[url=.+?\])(.+?)(?:\[\/url\])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matches = fullBBcodeRegex.Matches(text).Cast<Match>().Select(m => m.Value).ToList();




            var paragraph = new Paragraph();
            flowDocument.Blocks.Add(paragraph);

            int i = 0;
            foreach (var segment in fullBBcodeRegex.Split(text))
            {
                string centerword = null;




                if (matches.Contains(segment))
                {
                    foreach (var center in centerwordBbRegex.Split(segment))
                    {
                        if (center != "")
                        {
                            centerword = center;
                        }
                    }


                    var hyperlink = new Hyperlink(new Run(centerword))
                    {
                        NavigateUri = new Uri(urlList[i]),
                    };
                    var count = i;
                    hyperlink.RequestNavigate += (sender, args) => Process.Start(urlList[count]);

                    paragraph.Inlines.Add(hyperlink);
                    i++;
                }
                else
                {
                    paragraph.Inlines.Add(new Run(segment));
                }
            }
            return flowDocument;
        }
    }
}

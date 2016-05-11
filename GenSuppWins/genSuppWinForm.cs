using System;
using System.Collections.Generic;
using System.ComponentModel;
using CsQuery;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace GenSuppWins
{
    public partial class suppWinForm : Form
    {
        private const string _suppWinTop = @"<!DOCTYPE HTML>
<html>
<head>
<meta charset=""utf-8"">
<meta http-equiv=""X-UA-Compatible"" content=""IE=edge,chrome=1"" >
<meta name = ""viewport"" content=""width=device-width, initial-scale=1.0"">
<title>_DL_TITLE_</title>
<link href = ""../../../../css/boilerplate.css"" media=""screen"" rel=""stylesheet"" type=""text/css"" >
<link href = ""../../../../css/jquery-ui-custom.css"" media=""screen"" rel=""stylesheet"" type=""text/css"" >
<link href = ""../../../../css/manuscript.css"" media=""screen"" rel=""stylesheet"" type=""text/css"" >
<link href = ""../../../../css/digfir_ebook_fw.css"" media=""screen"" rel=""stylesheet"" type=""text/css"" >
<link href = ""../../../../css/ips9e.css"" media=""screen"" rel=""stylesheet"" type=""text/css"" >
<link href = ""../../../../css/ips9e_ch_DL_CHAPTER_NUMBER_.css"" media=""screen"" rel=""stylesheet"" type=""text/css"" >
<link href = ""../../../supp_win.css"" media=""screen"" rel=""stylesheet"" type=""text/css"" >
</head>
<body id=""supp_win"">
<div id=""manuscript"" class=""_DL_MAN_CLASS_"">
<div data-type=""section"" data-block_type=""section"">";

        private const string _suppWinBottom = @"</div>
</div>
<script type = ""text/javascript"" src=""../../../../js/utilities.js""></script>
<script type = ""text/javascript"" src=""../../../../js/query_types.js""></script>
<script type = ""text/javascript"" src=""../../../../js/player.js""></script>
<script type = ""text/javascript"" src=""../../../../js/jquery.js""></script>
<script type = ""text/javascript"" src=""../../../../js/jquery-ui-1.8.16.custom.min.js""></script>
<script type = ""text/javascript"" src=""../../../../js/jquery_extensions.js""></script>
<script type = ""text/javascript"" src=""../../../../js/swfobject.js""></script>
<script type = ""text/javascript"" src=""http://admin.brightcove.com/js/BrightcoveExperiences.js""></script>
<script type = ""text/javascript"" src=""../../../../js/digfir_ebook_fw.js""></script>
<script type = ""text/javascript"" src=""../../../../js/ips9e.js""></script>
<script type = ""text/javascript"" src=""../../../../js/ips9e_ch_DL_CHAPTER_NUMBER_.js""></script>
<script type = ""text/javascript"" src=""../../../supp_win.js""></script>
<script type = ""text/javascript"">

//<!--
$(window).ready(function ()
    {
        player.initialize('52c6e83c757a2ef70c000000');
    });
//-->
</script>
</body>
</html>";

        
        // source HTML files
        private string srcDir = @"c:\Users\Meade\Source\Repos\ips9e-html\ips9e-html\ips9e-html\ebook";
        private string[] srcHtmlFiles;
        // supplemental output directory -- will be adjusted per chapter
        private string baseSuppWinDir;
        private string suppWinDir;
        // supplemental file base name, the prefix of all HTML page files.
        // *could* be derived from file contents or source HTML files
        private const string bookId = "ips9e";
        // List to contain HTML tag block for use in supplemental windows. Extracted from source HTML files.
        private List<string> tagList = new List<string>();
        private const string btFig = "FIGURE";
        private const string btUnfig = "UN-FIGURE";
        private const string btTable = "TABLE";
        private const string btUntable = "UN-TABLE";
        private const string btQprob = "q_prob";
        private const string btQcyu = "q_cyu";
        private const string dtQuestion = "question";
        private const string subChapNum = "_DL_CHAPTER_NUMBER_";
        private const string subTitle = "_DL_TITLE_";
        private const string subManClass = "_DL_MAN_CLASS_";
        // edited copies of constants above
        private string suppWinTop;
        private string suppWinBottom;
        private const string btExample = "EXP";
        // provisional chapter number -- encapsulate more than this!
        private string chapnum;

        public suppWinForm()
        {
            InitializeComponent();
        }

        // All configuration settings may be moved to a configuration file
        // There can be several config files, one for each book, and the interface might
        // just allow for the selection of one of them.
        private void SourceDirBtn_Click(object sender, EventArgs e)
        {
            selDirDialog.Description = "Select HTML Source Directory";
            // for now, can source dir for convenience
            selDirDialog.SelectedPath = srcDir;
            // but allow it to be changed as well
            if (DialogResult.OK == selDirDialog.ShowDialog())
            {
                srcDir = selDirDialog.SelectedPath;
                SrcDirLabel.Text = srcDir;
            }
        }

        private void genSWsBtn_Click(object sender, EventArgs e)
        {
            if (srcDir == null) return;

            // Here's code for chapter files, but will eventually need to include appendices, FM/BM, etc.

            srcHtmlFiles = Directory.GetFiles(srcDir, bookId + "_ch*.html");

            foreach (string secfile in srcHtmlFiles)
            {
                // Set base directory for all supp wins. Some will be put in subdirectories.
                setBaseSuppWinDir(secfile);
                
                // figures use subdirectory
                suppWinDir = baseSuppWinDir + @"figures\";
                // textBox1.Text += secfile + Environment.NewLine;
                // generate numbered figure supplemental window files
                genFigSuppWins(secfile, btFig);
                // generate unnumbered figure supplemental window files
                // genFigSuppWins(secfile, btUnfig);

                // Process tables
                // use table subdirectory
                suppWinDir = baseSuppWinDir + @"tables\";
                // generate numbered figure supplemental window files
                genTblSuppWins(secfile, btTable);
                // generate unnumbered figure supplemental window files
                //genTblSuppWins(secfile, btUntable);

                // Process questions
                // use questions subdirectory
                suppWinDir = baseSuppWinDir + @"exercises\";
                // generate question supplemental window files
                // Problems section
                genQSuppWins(secfile, dtQuestion);
                // genQSuppWins(secfile, btQprob);
                // Check Your Understanding boxes
                // genQSuppWins(secfile, btQcyu);

                // Process examples
                // use examples subdirectory
                suppWinDir = baseSuppWinDir + @"examples\";
                // generate example supplemental window files
                genExpSuppWins(secfile, btExample);
            }

        }

        // used to set global class variable, but needs changing for different supp wins
        private void setBaseSuppWinDir(string HtmlFile)
        {
            // get chapter number from filename
            var fname = Path.GetFileNameWithoutExtension(HtmlFile);
            Regex rgx = new Regex(@".*?_ch0?(\d+)_\d*");
            chapnum = rgx.Replace(fname, "$1");
            // get source file path, add on target path
            baseSuppWinDir = Path.GetDirectoryName(HtmlFile) + @"\asset\ch" + chapnum + @"\supp_wins\";
        }

        private void genFigSuppWins(string HtmlFile, string block_type)
        {
            // create the DOM by reading file
            CQ dom = CQ.CreateFromFile(HtmlFile);

            // get section block type
            var secType = dom["[data-type='section']"].Attr("data-block_type");
            secType = String.Format("data-block_type=\"{0}\"", secType);

            // select with JQuery methods
            var figBlocks = dom["div[data-block_type='" + block_type + "']"];

            int figCnt = figBlocks.Count();

            // generate supplemental window files
            int imgCnt = 0;

            foreach (var figEl in figBlocks)
            {
                // NAME SUPPLEMENTAL WINDOW FILE
                // find image name to use for file name

                var attrs = figEl.Attributes;
                var filename = attrs["data-filename"];

                // extract the HTML
                var figBlock = figEl.OuterHTML;

                // EDIT IMAGE PATHS
                // make image paths relative (just remove path -- in same directory in this case)
                // NOTE: Maybe save path for later use in placing files to chapter directories,
                // which are canned here
                figBlock = figBlock.Replace("asset/ch1/", "../../");

                // get figure number for supp win title
                Regex rgx = new Regex(@"figure_\d+_(\d+).html");
                string fignum = rgx.Replace(filename, "$1");
                string title = "Figure";
                if (fignum.Length > 0)
                {
                    title = String.Format("{0} {1}-{2}", title, chapnum, fignum);
                }

                // Edit "constant" header and footer for title and chapter CSS and JS file names
                suppWinTop = _suppWinTop.Replace(subTitle, title);
                suppWinTop = suppWinTop.Replace(subChapNum, chapnum);
                suppWinTop = suppWinTop.Replace(subManClass, "figure");
                suppWinTop = suppWinTop.Replace(@"data-block_type=""section""", secType);
                suppWinBottom = _suppWinBottom.Replace(subChapNum, chapnum);

                // surround figure code with supplemental file HTML
                figBlock = suppWinTop + Environment.NewLine + figBlock + Environment.NewLine + suppWinBottom;

                // and write to target directory
                File.WriteAllText(suppWinDir + filename, figBlock);
                imgCnt++;
            }
            textBox1.Text += String.Format("{0} {1} files written{2}", imgCnt, block_type, Environment.NewLine);
        }

        private void genTblSuppWins(string HtmlFile, string block_type)
        {
            // create the DOM by reading file
            CQ dom = CQ.CreateFromFile(HtmlFile);

            // get section block type
            var secType = dom["[data-type='section']"].Attr("data-block_type");
            // just can EXP for now, for styling
            secType = String.Format("data-block_type=\"{0}\"", secType);

            // select with JQuery methods
            var tblBlocks = dom["div[data-block_type='" + block_type + "']"];

            int tblCnt = 0;

            // generate supplemental window files
            foreach (var tblEl in tblBlocks)
            {
                // NAME SUPPLEMENTAL WINDOW FILE
                // filename is found in the data-filename attribute of the main table div
                var attrs = tblEl.Attributes;
                var filename = attrs["data-filename"];

                // get table number for supp win title
                Regex rgx = new Regex(@"table_\d+_(\d+).html");
                string tblnum = rgx.Replace(filename, "$1");
                string title = "Table";
                if (tblnum.Length > 0)
                {
                    title = String.Format("{0} {1}-{2}", title, chapnum, tblnum);
                }

                // extract the HTML
                var tblBlock = tblEl.OuterHTML;

                // EDIT IMAGE PATHS - reference parent directory
                rgx = new Regex(@"(src="")asset/ch\d+/");
                tblBlock = rgx.Replace(tblBlock, "$1../");

                // Edit "constant" header and footer for title and chapter CSS and JS file names
                suppWinTop = _suppWinTop.Replace(subTitle, title);
                suppWinTop = suppWinTop.Replace(subChapNum, chapnum);
                suppWinTop = suppWinTop.Replace(subManClass, "table");
                suppWinTop = suppWinTop.Replace(@"data-block_type=""section""", secType);
                suppWinBottom = _suppWinBottom.Replace(subChapNum, chapnum);

                // surround table code with supplemental file HTML
                tblBlock = suppWinTop + Environment.NewLine + tblBlock + Environment.NewLine + suppWinBottom;

                // and write to target directory
                File.WriteAllText(suppWinDir + filename, tblBlock);
                tblCnt++;
            }

            textBox1.Text += String.Format("{0} {1} files written{2}", tblCnt, block_type, Environment.NewLine);
        }

        private void genQSuppWins(string HtmlFile, string block_type)
        {
            // create the DOM by reading file
            CQ dom = CQ.CreateFromFile(HtmlFile);

            // get section block type
            var secType = dom["[data-type='section']"].Attr("data-block_type");
            secType = String.Format("data-block_type=\"{0}\"", secType);

            // select with JQuery methods
            var qBlocks = dom["div[data-type='" + block_type + "']"];

            int qCnt = 0;

            // generate supplemental window files
            foreach (var qEl in qBlocks)
            {
                // NAME SUPPLEMENTAL WINDOW FILE
                // filename is found in the block type of the main table div
                var attrs = qEl.Attributes;
                var filename = attrs["data-block_type"];
                if (filename == null)
                {
                    filename = "MISSING_FILENAME.html";
                }

                // extract the HTML
                var qBlock = qEl.OuterHTML;

                // EDIT IMAGE PATHS - reference parent directory
                Regex rgx = new Regex(@"(src="")asset/ch\d+/");
                qBlock = rgx.Replace(qBlock, "$1../../../");

                // get question number for supp win title
                rgx = new Regex(@"exercise_\d+_(\d+).html");
                string qtnum = rgx.Replace(filename, "$1");
                string title = "Exercise";
                if (qtnum.Length > 0)
                {
                    title = String.Format("{0} {1}-{2}", title, chapnum, qtnum);
                }

                // Edit "constant" header and footer for title and chapter CSS and JS file names
                suppWinTop = _suppWinTop.Replace(subTitle, title);
                suppWinTop = suppWinTop.Replace(subChapNum, chapnum);
                suppWinTop = suppWinTop.Replace(subManClass, "exercise");
                suppWinTop = suppWinTop.Replace(@"data-block_type=""section""", secType);
                suppWinBottom = _suppWinBottom.Replace(subChapNum, chapnum);

                // surround table code with supplemental file HTML
                qBlock = suppWinTop + Environment.NewLine + qBlock + Environment.NewLine + suppWinBottom;

                // and write to target directory
                File.WriteAllText(suppWinDir + filename, qBlock);
                qCnt++;
            }

            textBox1.Text += String.Format("{0} {1} files written{2}", qCnt, block_type, Environment.NewLine);
        }
        private void genExpSuppWins(string HtmlFile, string block_type)
        {
            // create the DOM by reading file
            CQ dom = CQ.CreateFromFile(HtmlFile);

            // get section block type
            var secType = dom["[data-type='section']"].Attr("data-block_type");
            secType = String.Format("data-block_type=\"{0}\"", secType);

            // select with JQuery methods
            var expBlocks = dom["div[data-block_type='" + block_type + "']"];

            int expCnt = 0;

            // generate supplemental window files
            foreach (var expEl in expBlocks)
            {
                // NAME SUPPLEMENTAL WINDOW FILE
                // filename is found in the data-filename attribute of the main example div
                var attrs = expEl.Attributes;
                var filename = attrs["data-filename"];

                // extract the HTML
                var expBlock = expEl.OuterHTML;

                // EDIT IMAGE PATHS - reference parent directory
                // Regex rgx = new Regex(@"(src="")asset/ch\d+/");
                // expBlock = rgx.Replace(expBlock, "$1../../../../");

                // get example number for supp win title
                Regex rgx = new Regex(@"example_\d+_(\d+).html");
                string qtnum = rgx.Replace(filename, "$1");
                string title = "Example";
                if (qtnum.Length > 0)
                {
                    title = String.Format("{0} {1}-{2}", title, chapnum, qtnum);
                }

                // Edit "constant" header and footer for title and chapter CSS and JS file names
                suppWinTop = _suppWinTop.Replace(subTitle, title);
                suppWinTop = suppWinTop.Replace(subChapNum, chapnum);
                suppWinTop = suppWinTop.Replace(subManClass, "example");
                suppWinTop = suppWinTop.Replace(@"data-block_type=""section""", secType);
                suppWinBottom = _suppWinBottom.Replace(subChapNum, chapnum);

                // surround table code with supplemental file HTML
                expBlock = suppWinTop + Environment.NewLine + expBlock + Environment.NewLine + suppWinBottom;

                // and write to target directory
                File.WriteAllText(suppWinDir + filename, expBlock);
                expCnt++;
            }

            textBox1.Text += String.Format("{0} {1} files written{2}", expCnt, block_type, Environment.NewLine);
        }

    }
}

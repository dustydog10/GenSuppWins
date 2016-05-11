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
        private string suppWinTop = @"<!DOCTYPE HTML>
<html>
<head>
<meta charset=""utf-8"">
<meta http-equiv=""X-UA-Compatible"" content=""IE=edge,chrome=1"" >
<meta name = ""viewport"" content=""width=device-width, initial-scale=1.0"">
<title>Figure</title>
<link href = ""../../css/boilerplate.css"" media=""screen"" rel=""stylesheet"" type=""text/css"" >
<link href = ""../../css/jquery-ui-custom.css"" media=""screen"" rel=""stylesheet"" type=""text/css"" >
<link href = ""../../css/manuscript.css"" media=""screen"" rel=""stylesheet"" type=""text/css"" >
<link href = ""../../css/digfir_ebook_fw.css"" media=""screen"" rel=""stylesheet"" type=""text/css"" >
<link href = ""../../css/krugmanwellsessentials4e.css"" media=""screen"" rel=""stylesheet"" type=""text/css"" >
<link href = ""../../css/krugmanwellsessentials4e_ch3.css"" media=""screen"" rel=""stylesheet"" type=""text/css"" >
</head>
<body id=""supp_win"">";

        private string suppWinBottom = @"<script type=""text/javascript"" src=""../../js/utilities.js""></script>
<script type = ""text/javascript"" src=""../../js/query_types.js""></script>
<script type = ""text/javascript"" src=""../../js/player.js""></script>
<script type = ""text/javascript"" src=""../../js/jquery.js""></script>
<script type = ""text/javascript"" src=""../../js/jquery-ui-1.8.16.custom.min.js""></script>
<script type = ""text/javascript"" src=""../../js/jquery_extensions.js""></script>
<script type = ""text/javascript"" src=""../../js/swfobject.js""></script>
<script type = ""text/javascript"" src=""http://admin.brightcove.com/js/BrightcoveExperiences.js""></script>
<script type = ""text/javascript"" src=""../../js/digfir_ebook_fw.js""></script>
<script type = ""text/javascript"" src=""../../js/krugmanwellsessentials4e.js""></script>
<script type = ""text/javascript"" src=""../../js/krugmanwellsessentials4e_ch3.js""></script>
<script type = ""text/javascript"">

\/\/<!--
$(window).ready(function ()
    {
        player.initialize('52c6e83c757a2ef70c000000');
    });
\/\/-->
</script>
</body>
</html>";

        
        // source HTML files
        private string srcDir = @"c:\Users\Meade\odrive\Google Drive devlions\krug_ess4e\html";
        private string[] srcHtmlFiles;
        // supplemental output directory -- will be adjusted per chapter
        private string baseSuppWinDir;
        private string suppWinDir;
        // supplemental file base name, the prefix of all HTML page files.
        // *could* be derived from file contents or source HTML files
        private const string bookId = "krugmanwellsessentials4e";
        // List to contain HTML tag block for use in supplemental windows. Extracted from source HTML files.
        private List<string> tagList = new List<string>();
        private const string btFig = "figure";
        private const string btUnfig = "un_figure";
        private const string btTable = "table";
        private const string btUntable = "un_table";
        private const string btQprob = "q_prob";
        private const string btQcyu = "q_cyu";
        private const string dtQuestion = "question";

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
            selDirDialog.SelectedPath = @"c:\Users\Meade\odrive\Google Drive devlions\krug_ess4e\html";
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

            // Determine the number of chapters and set up loop for each
            // *** alternately, process all files and just detect chapter number as each is processed.

            // Here's code for chapter files, but will eventually need to include appendices, FM/BM, etc.

            srcHtmlFiles = Directory.GetFiles(srcDir, bookId + "_ch*.html");
            //textBox1.Text = String.Join(Environment.NewLine, srcHtmlFiles);

            foreach (string secfile in srcHtmlFiles)
            {
                // Set base directory for all supp wins. Some will be put in subdirectories.
                setBaseSuppWinDir(secfile);
                // figures use the base supp win directory
                suppWinDir = baseSuppWinDir;

                // textBox1.Text += secfile + Environment.NewLine;
                // generate numbered figure supplemental window files
                //genFigSuppWins(secfile, btFig);
                // generate unnumbered figure supplemental window files
                //genFigSuppWins(secfile, btUnfig);

                // Process tables
                // use table subdirectory
                suppWinDir = baseSuppWinDir + @"tables\";
                // generate numbered figure supplemental window files
                //genTblSuppWins(secfile, btTable);
                // generate unnumbered figure supplemental window files
                //genTblSuppWins(secfile, btUntable);

                // Process questions
                // use questions subdirectory
                suppWinDir = baseSuppWinDir + @"questions\";
                // generate question supplemental window files
                // Problems section
                // genQSuppWins(secfile, dtQuestion);
                genQSuppWins(secfile, btQprob);
                // Check Your Understanding boxes
                genQSuppWins(secfile, btQcyu);
            }

        }

        // used to set global class variable, but needs changing for different supp wins
        private void setBaseSuppWinDir(string HtmlFile)
        {
            // get chapter number from filename
            var fname = Path.GetFileNameWithoutExtension(HtmlFile);
            Regex rgx = new Regex(@".*?_ch0?(\d+)_\d*");
            string chapnum = rgx.Replace(fname, "$1");
            // get source file path, add on target path
            suppWinDir = Path.GetDirectoryName(HtmlFile) + @"\asset\ch" + chapnum + @"\";
        }

        private void genFigSuppWins(string HtmlFile, string block_type)
        {
            // create the DOM by reading file
            CQ dom = CQ.CreateFromFile(HtmlFile);

            // select with JQuery methods
            var figBlocks = dom["div[data-block_type='" + block_type + "']"];
            // this is the only way I could get the img blocks out of the DOM. They seem synchronized
            // with the figure block collection (above), so this seems to work, but it would be better
            // to extract this info from the figBlocks themselves, one by one.
            var imgs = figBlocks.Find("img");
            // note: this Find() works here, but it won't work on the figEl vars below

            int figCnt = figBlocks.Count();

            // generate supplemental window files
            int imgCnt = 0;

            foreach (var figEl in figBlocks)
            {
                // NAME SUPPLEMENTAL WINDOW FILE
                // find image name to use for file name
                // NOTE: Maybe save path for later use in placing files to chapter directories,
                // which are canned here
                var img = imgs[imgCnt];
                string imgname = img.Attributes["src"];
                imgname = imgname.Replace("asset/ch3/", "");
                var filename = imgname.Replace("jpg", "html");

                // extract the HTML
                var figBlock = figEl.OuterHTML;

                // EDIT IMAGE PATHS
                // make image paths relative (just remove path -- in same directory in this case)
                // NOTE: Maybe save path for later use in placing files to chapter directories,
                // which are canned here
                figBlock = figBlock.Replace("asset/ch3/", "");

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

            // select with JQuery methods
            var tblBlocks = dom["div[data-block_type='" + block_type + "']"];

            int tblCnt = 0;

            // generate supplemental window files
            foreach (var tblEl in tblBlocks)
            {
                // NAME SUPPLEMENTAL WINDOW FILE
                // the data-href attribute holds the whole path with file name relative to the source HTML directory
                // available only for numbered tables
                string tblhref;

                if (block_type == btTable)
                {
                    tblhref = tblEl.Attributes["data-href"];
                    tblhref = @"/" + tblhref;
                }
                // for unnumberd tables, file name must be derived from the ID
                // maybe a better way can be found, like including the data-href in the XML => HTML, as above
                else
                {
                    var tblId = tblEl.Attributes["id"];
                    tblhref = "asset/ch";
                    Regex idrgx = new Regex(@"[^-]+-ch(\d+)-untab-(\d+)");
                    // krugmanwellsessentials4e-ch3-untab-4
                    // add the chapter number to the path
                    var tblChap = idrgx.Replace(tblId, "$1");
                    var tblNumber = idrgx.Replace(tblId, "$2");
                    tblhref = @"/asset/ch" + tblChap + @"/tables/untable_" + tblChap + "_" + tblNumber + ".html";
                }

                // extract the HTML
                var tblBlock = tblEl.OuterHTML;

                // EDIT IMAGE PATHS - reference parent directory
                Regex rgx = new Regex(@"(src="")asset/ch\d+/");
                tblBlock = rgx.Replace(tblBlock, "$1../");

                // surround table code with supplemental file HTML
                tblBlock = suppWinTop + Environment.NewLine + tblBlock + Environment.NewLine + suppWinBottom;

                // EDIT CSS and JS PATHS - one more level deep now
                tblBlock = tblBlock.Replace("../../", "../../../");

                // and write to target directory
                File.WriteAllText(srcDir + tblhref, tblBlock);
                tblCnt++;
            }

            textBox1.Text += String.Format("{0} {1} files written{2}", tblCnt, block_type, Environment.NewLine);
        }

        private void genQSuppWins(string HtmlFile, string block_type)
        {
            // create the DOM by reading file
            CQ dom = CQ.CreateFromFile(HtmlFile);

            // select with JQuery methods
            var qBlocks = dom["div[data-block_type='" + block_type + "']"];

            int qCnt = 0;

            // generate supplemental window files
            foreach (var qEl in qBlocks)
            {
                // NAME SUPPLEMENTAL WINDOW FILE
                // The question id holds the chapter and question number info, as well as the type of question
                string qhref;

                var qId = qEl.Attributes["id"];
                qhref = "asset/ch";
                Regex idrgx = new Regex(@"[^-]+-ch(\d+)-((cyu)?question)-(\d+)");
                // krugmanwellsessentials4e-ch3-cyuquestion-1
                // add the chapter number to the path
                string qChap = idrgx.Replace(qId, "$1");
                string qtype = idrgx.Replace(qId, "$2");
                qtype = qtype.Replace("cyuquestion", "cyu_question");
                string qNumber = idrgx.Replace(qId, "$4");
                qhref = @"/asset/ch" + qChap + @"/questions/" + qtype + "_" + qNumber + ".html";

                // extract the HTML
                var qBlock = qEl.OuterHTML;

                // EDIT IMAGE PATHS - reference parent directory
                Regex rgx = new Regex(@"(src="")asset/ch\d+/");
                qBlock = rgx.Replace(qBlock, "$1../");

                // surround table code with supplemental file HTML
                qBlock = suppWinTop + Environment.NewLine + qBlock + Environment.NewLine + suppWinBottom;

                // EDIT CSS and JS PATHS - one more level deep now
                qBlock = qBlock.Replace("../../", "../../../");

                // and write to target directory
                File.WriteAllText(srcDir + qhref, qBlock);
                qCnt++;
            }

            textBox1.Text += String.Format("{0} {1} files written{2}", qCnt, block_type, Environment.NewLine);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using CsQuery;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
        // supplemental output directory
        private string suppWinDir;
        // supplemental file base name, the prefix of all HTML page files.
        private const string bookId = "krugmanwellsessentials4e";
        // List to contain HTML tag block for use in supplemental windows. Extracted from source HTML files.
        private List<string> tagList = new List<string>();
        private const string btFig = "figure";
        private const string btUnfig = "un_figure"; 

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

            // Here's code for chapter 3, but must adapt to above

            srcHtmlFiles = Directory.GetFiles(srcDir, bookId + "_ch3_*.html");
            //textBox1.Text = String.Join(Environment.NewLine, srcHtmlFiles);

            // Another canned value for now. Assumes directory structure, too. Will be explicitly defined
            // in config files in the future.
            suppWinDir =  srcDir + @"\asset\ch3\";

            // Process just one section for now. ch3_3 has figures in it.
            string htmlFileName = srcDir + @"\krugmanwellsessentials4e_ch3_3.html";

            // generate numbered figure supplemental window files
            genSuppWins(htmlFileName, btFig);
            // generate unnumbered figure supplemental window files
            genSuppWins(htmlFileName, btUnfig);
        }

        private void genSuppWins(string HtmlFile, string block_type)
        {
            // create the DOM by reading file
            CQ dom = CQ.CreateFromFile(HtmlFile);

            // select with JQuery methods
            var figBlocks = dom["div[data-block_type='" + block_type + "']"];
            // this is the only way I could get the img blocks out of the DOM. They seem synchronized
            // with the figure block collection (above), so this seems to work, but it would be better
            // to extract this info from the figBlocks themselves, one by one.
            var imgs = figBlocks.Find("img");
            // this Find() works here, but it won't work on the figEl vars below

            int figCnt = figBlocks.Count();

            // generate supplemental window files
            int imgCnt = 0;

            foreach (var figEl in figBlocks)
            {
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

                // NAME SUPPLEMENTAL WINDOW FILE


                // and write to target directory
                File.WriteAllText(suppWinDir + filename, figBlock);
                // File.WriteAllText(suppWinDir + @"\testSuppWin" + i.ToString() + block_type + ".html", figBlock);
                imgCnt++;
            }
            textBox1.Text += String.Format("{0} {1} files written{2}", imgCnt, block_type, Environment.NewLine);
        }
    }
}

using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;


namespace markdown2html.NET
{
    public partial class window: Form
    {
        public window()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Markdown File",
                Filter = "Markdown Files (*.md)|*.md|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = openFileDialog.FileName;
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFilePath.Text) || !File.Exists(txtFilePath.Text))
            {
                MessageBox.Show("Please select a valid Markdown file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string markdownText = File.ReadAllText(txtFilePath.Text);
                string htmlContent = ConvertMarkdownToHtml(markdownText);

                // Save the HTML file
                string htmlFilePath = Path.ChangeExtension(txtFilePath.Text, ".html");
                File.WriteAllText(htmlFilePath, htmlContent);

                // Load the HTML content into WebBrowser
                webBrowserPreview.DocumentText = htmlContent;

                MessageBox.Show($"Conversion successful!\nSaved to: {htmlFilePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Conversion Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void rtbPreview_TextChanged(object sender, EventArgs e)
        {

        }


        private string ConvertMarkdownToHtml(string markdown)
        {
            StringBuilder html = new StringBuilder();

            // Define CSS styles
            string cssStyles = @"
        <style>
            body {
                font-family: Arial, sans-serif;
                background-color: #b3b3b3;
                text-align: left;
                padding-left: 10%;
            }
            .img {
                width: 80%;
            }
        </style>";

            // Start building the HTML
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset=\"UTF-8\">");  // Ensure UTF-8 support
            html.AppendLine(cssStyles);  // Add CSS styles
            html.AppendLine("</head>");
            html.AppendLine("<body>");

            string[] lines = markdown.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                // Convert headers
                if (trimmedLine.StartsWith("# "))
                    html.AppendLine($"<h1>{trimmedLine.Substring(2)}</h1>");
                else if (trimmedLine.StartsWith("## "))
                    html.AppendLine($"<h2>{trimmedLine.Substring(3)}</h2>");
                else if (trimmedLine.StartsWith("### "))
                    html.AppendLine($"<h3>{trimmedLine.Substring(4)}</h3>");
                else if (trimmedLine.StartsWith("#### "))
                    html.AppendLine($"<h4>{trimmedLine.Substring(5)}</h4>");
                else if (trimmedLine.StartsWith("##### "))
                    html.AppendLine($"<h5>{trimmedLine.Substring(6)}</h5>");
                else if (trimmedLine.StartsWith("###### "))
                    html.AppendLine($"<h6>{trimmedLine.Substring(7)}</h6>");

                // Convert unordered lists
                else if (trimmedLine.StartsWith("- ") || trimmedLine.StartsWith("* "))
                    html.AppendLine($"<li>{trimmedLine.Substring(2)}</li>");

                // Convert ordered lists
                else if (trimmedLine.Length > 2 && char.IsDigit(trimmedLine[0]) && trimmedLine[1] == '.')
                    html.AppendLine($"<li>{trimmedLine.Substring(2)}</li>");

                // Convert blockquotes
                else if (trimmedLine.StartsWith("> "))
                    html.AppendLine($"<blockquote>{trimmedLine.Substring(2)}</blockquote>");

                // Convert images ![alt](url)
                else if (Regex.IsMatch(trimmedLine, @"!\[(.*?)\]\((.*?)\)"))
                {
                    trimmedLine = Regex.Replace(trimmedLine, @"!\[(.*?)\]\((.*?)\)", "<img class='img' src='$2' alt='$1'>");
                    html.AppendLine(trimmedLine);
                }

                // Convert normal paragraphs (apply bold & italic inside)
                else if (!string.IsNullOrWhiteSpace(trimmedLine))
                {
                    // Convert bold (**bold text**)
                    trimmedLine = Regex.Replace(trimmedLine, @"\*\*(.*?)\*\*", "<b>$1</b>");

                    // Convert italic (*italic text*)
                    trimmedLine = Regex.Replace(trimmedLine, @"\*(.*?)\*", "<i>$1</i>");

                    html.AppendLine($"<p>{trimmedLine}</p>");
                }
            }

            // Close the body and HTML
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        private void txtFilePath_TextChanged(object sender, EventArgs e)
        {

        }

        private void window_Load(object sender, EventArgs e)
        {

        }
    }
}

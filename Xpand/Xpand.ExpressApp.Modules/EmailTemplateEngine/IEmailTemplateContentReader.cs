namespace Xpand.EmailTemplateEngine {
    public interface IEmailTemplateContentReader {
        string Read(string templateName, string suffix);
    }
}
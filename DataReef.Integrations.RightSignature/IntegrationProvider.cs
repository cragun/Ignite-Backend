using DataReef.TM.Models.DTOs.Signatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace DataReef.Integrations.RightSignature
{
    public class IntegrationProvider
    {
        private readonly string _baseUrl;
        private readonly string _apiToken;

        public IntegrationProvider(string inputApiToken)
        {
            _baseUrl = "https://rightsignature.com";
            _apiToken = inputApiToken;
        }

        public XDocument SendDocument(string documentUrl, string subject, IDictionary<string, string> tags = null, string callbackURL = null, ICollection<Recipient> recipients = null, int? expiresInDays = null)
        {
            string requestPath = "/api/documents.xml";
            XElement rootNode = new XElement("document");
            XDocument xml = new XDocument(rootNode);

            rootNode.Add(new XElement("subject", subject));

            XElement documentDataNode = new XElement("document_data");
            documentDataNode.Add(new XElement("type", "url"));
            documentDataNode.Add(new XElement("value", documentUrl));
            rootNode.Add(documentDataNode);

            rootNode.Add(CreateRecipientsXML(recipients));

            if (tags != null) rootNode.Add(CreateTagsXML(tags));

            if (expiresInDays != null) rootNode.Add(new XElement("expires_in", String.Format("{0} days", expiresInDays))); // Must be 2, 5, 15, or (default) 30 days
            rootNode.Add(new XElement("action", "send"));
            rootNode.Add(new XElement("use_text_tags", "true"));
            if (callbackURL != null) rootNode.Add(new XElement("callback_location", callbackURL));

            // Sends the HTTP Request and returns the xml response as XDocument
            return ParseResponseAsXML(HttpRequest("POST", requestPath, xml.ToString()));
        }

        public string GetDocumentId(XDocument document)
        {
            return document.Element("document").Element("guid").Value;
        }

        public string GetDocumentStatus(XDocument document)
        {
            return document.Element("document").Element("status").Value;
        }

        public ICollection<SignerLink> GetSignerLinks(string documentId, string redirectLocation)
        {
            List<SignerLink> signerLinks = new List<SignerLink>();
            string urlPath = string.Format("/api/documents/{0}/signer_links.xml?redirect_location={1}", documentId, redirectLocation);

            var signerLinksResponse = ParseResponseAsXML(HttpRequest("GET", urlPath, null));

            foreach (XElement element in signerLinksResponse.Element("document").Element("signer-links").Elements("signer-link"))
            {
                signerLinks.Add(new SignerLink {
                    Name = (string)element.Element("name"),
                    Role = (string)element.Element("role"),
                    SignerToken = (string)element.Element("signer-token")
                });
            }
            return signerLinks;
        }

        public void SendEmailReminder(string contractID)
        {
            var requestPath = String.Format("/api/documents/{0}/send_reminders.xml", contractID);
            XElement rootNode = new XElement("document");
            XDocument xml = new XDocument(rootNode);
            HttpRequest("POST", requestPath, xml.ToString());
        }

        public string GetSignedDocumentURL(string documentId)
        {
            string urlPath = string.Format("/api/documents/{0}.xml", documentId);
            var documentDetailsResponse = ParseResponseAsXML(HttpRequest("GET", urlPath, null));
            return documentDetailsResponse.Element("document").Element("signed-pdf-url").Value;
        }

        private XElement CreateRecipientsXML(ICollection<Recipient> recipients)
        {
            XElement recipientsNode = new XElement("recipients");
            if (recipients == null || recipients.Count == 0)
            {
                XElement recipientNode = new XElement("recipient");
                recipientNode.Add(new XElement("name", "homeowner"));
                recipientNode.Add(new XElement("email", "noemail@rightsignature.com"));
                recipientNode.Add(new XElement("role", "signer"));
                recipientsNode.Add(recipientNode);
                recipientNode = new XElement("recipient");
                recipientNode.Add(new XElement("name", "sales rep"));
                recipientNode.Add(new XElement("email", "noemail@rightsignature.com"));
                recipientNode.Add(new XElement("role", "signer"));
                recipientsNode.Add(recipientNode);

                recipientNode = new XElement("recipient");
                recipientNode.Add(new XElement("is_sender", "true"));
                recipientNode.Add(new XElement("role", "cc"));
                recipientsNode.Add(recipientNode);
            }
            else
            {
                foreach (var recipient in recipients)
                {
                    XElement recipientNode = new XElement("recipient");
                    recipientNode.Add(new XElement("name", recipient.Name));
                    recipientNode.Add(new XElement("email", recipient.Email));
                    recipientNode.Add(new XElement("role", recipient.Role));
                    if (recipient.IsSender) recipientNode.Add(new XElement("is_sender", "true"));
                    recipientsNode.Add(recipientNode);
                }
            }
            return recipientsNode;
        }

        private XElement CreateTagsXML(IDictionary<string, string> tags)
        {
            XElement tagsNode = new XElement("tags");
            foreach (KeyValuePair<string, string> tag in tags)
            {
                XElement tagNode = new XElement("tag");
                tagNode.Add(new XElement("name", tag.Key));
                if (tag.Value != null) tagNode.Add(new XElement("value", tag.Value));
                tagsNode.Add(tagNode);
            }
            return tagsNode;
        }

        private XDocument ParseResponseAsXML(HttpWebResponse response)
        {
            using (XmlReader xmlReader = XmlReader.Create(response.GetResponseStream()))
            {
                XDocument xdoc = XDocument.Load(xmlReader);
                xmlReader.Close();
                return xdoc;
            }
        }

        private HttpWebResponse HttpRequest(string method, string path, string body = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(_baseUrl + path);

            // add API Token to Header, for authorization
            request.Headers.Add("api-token", _apiToken);
            request.Method = method;

            if (method.Equals("POST"))
            {
                request.ContentType = "text/xml";
                if (body != null)
                {
                    request.ContentLength = body.Length;
                    using (Stream writeStream = request.GetRequestStream())
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(body);
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }
            }

            var response = (HttpWebResponse)request.GetResponse();
            return response;
        }
    }
}

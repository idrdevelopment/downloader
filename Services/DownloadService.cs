namespace ISC.IDRDownloader.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;
    using ISC.IDRDownloader.Domain;
    using ISC.IDRDownloader.Services.Contracts;
    using ISC.IDRDownloader.Validation.Contracts;
    
    public class DownloadService : IDownloadService
    {
        private const string DateFormat = "yyyy-MM-ddTHH:mm:ss";
        private readonly IArgumentValidator argumentValidator;

        public DownloadService(IArgumentValidator argumentValidator)
        {
            this.argumentValidator = argumentValidator;
        }

        public StringBuilder ValidateArguments(string[] args)
        {
            return argumentValidator.Validate(args);
        }

        public Arguments Download(string[] args)
        {
            var arguments = PrepareArgs(args);
            PrepareUrls(arguments);

            var webClient = new MyWebClient(Configuration.ApiUrl, arguments.Username, arguments.Password);

            DownloadPartners(arguments, webClient);
            DownloadRelationships(arguments, webClient);
            DownloadReviews(arguments, webClient);
            UpdateChangesSince(arguments);

            return arguments;
        }

        private void UpdateChangesSince(Arguments arguments)
        {
            var myJson = GetJson();

            if (string.IsNullOrWhiteSpace(arguments.ChangesSince) &&
                string.IsNullOrWhiteSpace(arguments.ChangesSincePartners) &&
                string.IsNullOrWhiteSpace(arguments.ChangesSinceRelationships) &&
                string.IsNullOrWhiteSpace(arguments.ChangesSinceReviews))
            {
                myJson.ChangesSincePartners = DateTime.Now.ToString(DateFormat);
                myJson.ChangesSinceRelationships = DateTime.Now.ToString(DateFormat);
                myJson.ChangesSinceReviews = DateTime.Now.ToString(DateFormat);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(arguments.ChangesSincePartners) && (arguments.DownloadType == "all" || arguments.DownloadType == "entities"))
                {
                    myJson.ChangesSincePartners = DateTime.Now.ToString(DateFormat);
                }

                if (!string.IsNullOrWhiteSpace(arguments.ChangesSinceRelationships) && (arguments.DownloadType == "all" || arguments.DownloadType == "relationships"))
                {
                    myJson.ChangesSinceRelationships = DateTime.Now.ToString(DateFormat);
                }

                if (!string.IsNullOrWhiteSpace(arguments.ChangesSinceReviews) && (arguments.DownloadType == "all" || arguments.DownloadType == "reviewstatus"))
                {
                    myJson.ChangesSinceReviews = DateTime.Now.ToString(DateFormat);
                }
            }

            File.WriteAllText("my.json", JsonConvert.SerializeObject(myJson));
        }

        private MyJson GetJson()
        {
            var jsonFile = string.Empty;

            using (var reader = new StreamReader("my.json"))
            {
                jsonFile = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<MyJson>(jsonFile);
        }

        private Arguments PrepareArgs(string[] args)
        {
            var arguments = new Arguments();

            foreach (var arg in args)
            {
                var argParts = arg.Split('=');

                argParts[0] = argParts[0].TrimStart('-').ToLower();

                switch (argParts[0])
                {
                    case "username":
                        arguments.Username = argParts[1];
                        break;
                    case "password":
                        arguments.Password = argParts[1];
                        break;
                    case "type":
                        arguments.DownloadType = argParts[1];
                        break;
                    case "partnerid":
                        arguments.PartnerId = argParts[1];
                        break;
                    case "changessince":
                        arguments.ChangesSincePartners = GetFormattedDateAsString(argParts[1]);
                        arguments.ChangesSinceRelationships = GetFormattedDateAsString(argParts[1]);
                        arguments.ChangesSinceReviews = GetFormattedDateAsString(argParts[1]);
                        break;
                    case "changessincepartners":
                        arguments.ChangesSincePartners = GetFormattedDateAsString(argParts[1]);
                        break;
                    case "changessincerelationships":
                        arguments.ChangesSinceRelationships = GetFormattedDateAsString(argParts[1]);
                        break;
                    case "changessincereviews":
                        arguments.ChangesSinceReviews = GetFormattedDateAsString(argParts[1]);
                        break;
                    case "savepath":
                        arguments.SavePath = argParts[1];
                        break;
                }
            }

            return arguments;
        }

        private string GetFormattedDateAsString(string date)
        {
            return DateTime.Parse(date).ToString(DateFormat);
        }

        private void PrepareUrls(Arguments arguments)
        {
            arguments.EntityUrl = $"{Configuration.ApiUrl}/entities?searchType=userAccess";
            arguments.RelationshipUrl = $"{Configuration.ApiUrl}/entities/relationships/userAccess";
            arguments.ReviewUrl = $"{Configuration.ApiUrl}/entities/reviews/userAccess";

            AddPartnerToUrls(arguments);
            AddChangesSinceToUrls(arguments);

            var firstAmpersand = arguments.RelationshipUrl.IndexOf('&');

            if (firstAmpersand > 0)
            {
                arguments.RelationshipUrl = arguments.RelationshipUrl.Remove(firstAmpersand, 1);
                arguments.RelationshipUrl = arguments.RelationshipUrl.Insert(firstAmpersand, "?");
            }

            firstAmpersand = arguments.ReviewUrl.IndexOf('&');

            if (firstAmpersand > 0)
            {
                arguments.ReviewUrl = arguments.ReviewUrl.Remove(firstAmpersand, 1);
                arguments.ReviewUrl = arguments.ReviewUrl.Insert(firstAmpersand, "?");
            }
        }

        private void AddPartnerToUrls(Arguments arguments)
        {
            if (!string.IsNullOrWhiteSpace(arguments.PartnerId))
            {
                arguments.EntityUrl = $"{arguments.EntityUrl}&partnerId={arguments.PartnerId}";
                arguments.RelationshipUrl = $"{arguments.RelationshipUrl}&partnerId={arguments.PartnerId}";
                arguments.ReviewUrl = $"{arguments.ReviewUrl}&partnerId={arguments.PartnerId}";
            }
        }

        private void AddChangesSinceToUrls(Arguments arguments)
        {
            var myJson = GetJson();

            if (string.IsNullOrWhiteSpace(arguments.ChangesSince) && string.IsNullOrWhiteSpace(arguments.ChangesSincePartners))
            {
                arguments.ChangesSincePartners = myJson.ChangesSincePartners;
            }

            if (!string.IsNullOrWhiteSpace(arguments.ChangesSincePartners))
            {
                arguments.EntityUrl = $"{arguments.EntityUrl}&changesSince={arguments.ChangesSincePartners}";
            }

            if (string.IsNullOrWhiteSpace(arguments.ChangesSince) && string.IsNullOrWhiteSpace(arguments.ChangesSinceRelationships))
            {
                arguments.ChangesSinceRelationships = myJson.ChangesSinceRelationships;
            }

            if (!string.IsNullOrWhiteSpace(arguments.ChangesSinceRelationships))
            {
                arguments.RelationshipUrl = $"{arguments.RelationshipUrl}&changesSince={arguments.ChangesSinceRelationships}";
            }

            if (string.IsNullOrWhiteSpace(arguments.ChangesSince) && string.IsNullOrWhiteSpace(arguments.ChangesSinceReviews))
            {
                arguments.ChangesSinceReviews = myJson.ChangesSinceReviews;
            }

            if (!string.IsNullOrWhiteSpace(arguments.ChangesSinceReviews))
            {
                arguments.ReviewUrl = $"{arguments.ReviewUrl}&changesSince={arguments.ChangesSinceReviews}";
            }
        }

        private void DownloadPartners(Arguments arguments, MyWebClient webClient)
        {
            if (arguments.DownloadType == "all" || arguments.DownloadType == "entities")
            {
                var partners = webClient.Get<List<Partner>>(arguments.EntityUrl);

                using (var writer = new StreamWriter(GetFileName(arguments.SavePath, "Partners")))
                {
                    writer.WriteLine(Partner.Header);

                    foreach (var partner in partners)
                    {
                        writer.WriteLine(partner.ToString());
                    }
                }
            }
        }

        private void DownloadRelationships(Arguments arguments, MyWebClient webClient)
        {
            if (arguments.DownloadType == "all" || arguments.DownloadType == "relationships")
            {
                var relationships = webClient.Get<List<Relationship>>(arguments.RelationshipUrl);

                using (var writer = new StreamWriter(GetFileName(arguments.SavePath, "Relationship")))
                {
                    writer.WriteLine(Relationship.Header);

                    foreach (var relationship in relationships)
                    {
                        writer.WriteLine(relationship.ToString());
                    }
                }
            }
        }

        private void DownloadReviews(Arguments arguments, MyWebClient webClient)
        {
            if (arguments.DownloadType == "all" || arguments.DownloadType == "reviewstatus")
            {
                var reviews = webClient.Get<List<Review>>(arguments.ReviewUrl);

                using (var writer = new StreamWriter(GetFileName(arguments.SavePath, "Reviews")))
                {
                    writer.WriteLine(Review.Header);

                    foreach (var review in reviews)
                    {
                        writer.WriteLine(review.ToString());
                    }
                }
            }
        }

        private string GetFileName(string savePath, string startOfFileName)
        {
            return $"{savePath}\\{startOfFileName}{DateTime.Now.ToString("yyyyMMdd")}-{DateTime.Now.Ticks}.csv";
        }
    }
}

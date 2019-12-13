using DataReef.Integrations.Microsoft.PowerBI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Microsoft
{
    public class PowerBIPathsProvider
    {
        private static Dictionary<string, string> PBIPaths = new Dictionary<string, string>
        {
            { nameof(PBI_NewUser), "/beta/115e5a24-81e8-42c1-b5c2-da5e40585b85/datasets/763a920f-0531-43e3-b771-e4402be3eab1/rows?key=6aQJlCFXOMa5k4uyumnvGuHH4ukQFjcFqMS5URG4NKH%2F%2F0zTqil1KeRcEouuwJbkgvHcLot4KCxAIjDLq4W6xQ%3D%3D"},
            { nameof(PBI_ActiveUser),"/beta/115e5a24-81e8-42c1-b5c2-da5e40585b85/datasets/effac7bd-27f7-4ccb-97c5-a2365fee79f7/rows?key=lIYw%2BSbj8OLaZ25otkCRvoyw%2F2q20lyYmKMgFnqBhmD1IPm8kE9ljFMSz3BeWEBy7auxohDGoK1nJKyrF5yoog%3D%3D"},
            { nameof(PBI_ProposalCreated), "/beta/115e5a24-81e8-42c1-b5c2-da5e40585b85/datasets/c96e07ce-18bf-47f0-b2bb-9c56ac86a00c/rows?key=Ts%2BH67HbS3v90SZTs6DQnN0KI1y6oPoq4fLL5tQvDHD8CQLC1%2B4%2BByzoe%2Fpv8oJH%2FgJF4w0FaSSevnsHU0TuDw%3D%3D" },
            { nameof(PBI_ProposalSigned), "/beta/115e5a24-81e8-42c1-b5c2-da5e40585b85/datasets/0df687d1-f566-4abb-8269-ad7b280da970/rows?key=olg4SWmRzoIMPGCUlFQmbtn2ziHZblf5SFnNW3%2FLhMuBSBtc34qMBlHh7QAig%2FGIt4Jc2kR4FSd%2B95YFL8%2FroA%3D%3D"},
            { nameof(PBI_DispositionChanged),"/beta/115e5a24-81e8-42c1-b5c2-da5e40585b85/datasets/7bb53bb6-6f0f-41b4-952f-021f544e285b/rows?key=cU8akevF0EaqgvkZRN29kr6VJJmxePE%2BkX0MDKJdk8ShpXJCiPryCLhl8HBNz0o7CHw4WUKy5Fmq52uG7Vks5w%3D%3D"},
            { nameof(PBI_Consumption), "/beta/115e5a24-81e8-42c1-b5c2-da5e40585b85/datasets/847fbf71-52b7-4629-8b13-1b706e1a8f89/rows?key=Xg92%2FhCCUIszNpGmuhQA5CyBxigtGG3vzoFOx0KF0dJIWvF5gLHu2DF79dZ4mAR5Jjur0Vd8iaNtBW5hGJrKTA%3D%3D"},
            { nameof(PBI_RoofDrawing), "/beta/115e5a24-81e8-42c1-b5c2-da5e40585b85/datasets/4aa62a6c-7786-4cb0-8afa-23ae7151b10d/rows?key=MCmpMSonJt%2F6invO0bTMN37%2Bof8REsp2ZGiPAjJYyQTG8SxOtKS1DGcSxyTHdorck6lS8W1Ec8V8oBA0oLcLsg%3D%3D"}
        };

        public static string GetPath(string modelName)
        {
            if (PBIPaths.ContainsKey(modelName))
            {
                return PBIPaths[modelName];
            }

            return null;
        }
    }
}

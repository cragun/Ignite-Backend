using DataReef.TM.Api.Classes.Infrastructure;
using DataReef.TM.Models.DTOs.Signatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

public static class ModelExtensions
{
    public static async Task ProcessBinaryData(this DocumentSignRequest request, List<RequestFile> files)
    {
        if (request.DocumentData != null && request.DocumentData.Count > 0)
        {
            foreach (var doc in request.DocumentData)
            {
                var file = files.FirstOrDefault(f => f.Name.Equals(doc.Guid.ToString(), StringComparison.OrdinalIgnoreCase));
                if (file == null)
                {
                    throw new ApplicationException($"Couldn't find binary content for DocumentData: {doc.Guid}, of type: {doc.Type}");
                }

                doc.Content = await file.Content.ReadAsByteArrayAsync();
            }
        }

        if (request.UserInput != null && request.UserInput.Count > 0)
        {
            var group = request.UserInput.GroupBy(ui => ui.Guid);
            foreach (var ui in request.UserInput)
            {
                var file = files.FirstOrDefault(f => f.Name.Equals($"{ui.Guid}-{(int)ui.Type}", StringComparison.OrdinalIgnoreCase));
                file = file ?? files.FirstOrDefault(f => f.Name.Equals(ui.Guid.ToString(), StringComparison.OrdinalIgnoreCase));
                if (file == null)
                {
                    throw new ApplicationException($"Couldn't find binary content for UserInput: {ui.Guid}, of type: {ui.Type}");
                }

                ui.Content = await file.Content.ReadAsByteArrayAsync();

                if (ui.Validation?.Count > 0)
                {
                    ui.ValidationContent = new List<byte[]>();
                    foreach (var validation in ui.Validation)
                    {
                        file = files.FirstOrDefault(f => f.Name.Equals(validation.ToString(), StringComparison.OrdinalIgnoreCase));
                        if (file == null)
                        {
                            throw new ApplicationException($"Couldn't find binary content for Validation: {validation}!");
                        }
                        ui.ValidationContent.Add(await file.Content.ReadAsByteArrayAsync());
                    }
                }
            }
        }
    }
}

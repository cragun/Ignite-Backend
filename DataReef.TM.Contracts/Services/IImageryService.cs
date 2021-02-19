using DataReef.TM.Models.DTOs.Blobs;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    public interface IImageryService
    {
        /// <summary>
        /// Returns true if an orthogonal image ( Down ) exists at location
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        [OperationContract]
        bool HighResImageExistsAtLocation(double lat, double lon);

        /// <summary>
        /// Returns a list of image directions ( orientations ) Down, North, East, South,West
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        [OperationContract]
        List<string> AvailableOrientationsAtLocation(double lat, double lon);

        /// <summary>
        /// gets the hi-res image at the given lat / lon
        /// </summary>
        /// <param name="propertyID">property id for the purchase</param>
        /// <param name="top">Lat of the top cropping box side</param>
        /// <param name="left">Lon of the left cropping box side</param>
        /// <param name="bottom">Lat of the bottom cropping box side</param>
        /// <param name="right">Lon of the Right cropping box side</param>
        /// <param name="direction">Direction of Image.  Default is Down=Orthogonal</param>
        /// <returns></returns>
        [OperationContract]
        BlobModel PurchaseHighResImageAtCoordinates(Guid propertyID, double top, double left, double bottom, double right, string direction);


        /// <summary>
        /// returns the already purchased hi res image for a given property
        /// </summary>
        /// <param name="propertyID">property id for the purchase</param>
        /// <param name="top">Lat of the top cropping box side</param>
        /// <param name="left">Lon of the left cropping box side</param>
        /// <param name="bottom">Lat of the bottom cropping box side</param>
        /// <param name="right">Lon of the Right cropping box side</param>
        /// <param name="direction">Direction of Image.  Default is Down=Orthogonal</param>
        /// <returns></returns>
        [OperationContract]
        Task<BlobModel> GetExistingHiResImageForProperty(Guid propertyID, double top, double left, double bottom, double right, string direction);

        [OperationContract]
        void MigrateHiResImages();
    }
}

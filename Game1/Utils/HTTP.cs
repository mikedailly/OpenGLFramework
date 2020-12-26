// **********************************************************************************************************************
// 
// Copyright (c)2020, Ogre Games Ltd. All Rights reserved.
// 
// Date				Version		Comment
// ----------------------------------------------------------------------------------------------------------------------
// 26/12/2020		V1.0        1st version
// 
// **********************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Framework.Utils;

namespace Framework.Utils
{
    public delegate void DownloadFinished(string _name, byte[] _file);
    public static class HTTP
    {
        // *****************************************************************************************
        /// <summary>
        ///     Download a file
        /// </summary>
        /// <param name="_full_url"></param>
        // *****************************************************************************************
        public static async void Download( string _full_url, DownloadFinished _callback)
        {            
            HttpClient client = new HttpClient();

            try
            {
                // First connect to server and get the token - won't need this longer term
                using (HttpResponseMessage response = await client.GetAsync(_full_url) )
                {
                    using (HttpContent content = response.Content)
                    {
                        byte[] data = await content.ReadAsByteArrayAsync();
                        _callback(_full_url, data);
                    }
                }
            }catch (Exception ex)
            {
                // No actual connection error given, so fail with generic message
                // This should go away anyway once
                Log.WriteLine("Error: " + ex.Message);
            }
        }


    }
}

﻿// -----------------------------------------------------------------------
// <copyright file="ImageFactoryExtensions.cs" company="James South">
//     Copyright (c) James South.
//     Dual licensed under the MIT or GPL Version 2 licenses.
// </copyright>
// -----------------------------------------------------------------------

namespace ImageProcessor.Web
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using ImageProcessor.Processors;
    using ImageProcessor.Web.Config;
    using ImageProcessor.Web.Helpers;
    #endregion

    /// <summary>
    /// Extends the ImageFactory class to provide a fluent api.
    /// </summary>
    public static class ImageFactoryExtensions
    {
        /// <summary>
        /// Auto processes image files based on any querystring parameters added to the image path.
        /// </summary>
        /// <param name="factory">
        /// The current instance of the <see cref="T:ImageProcessor.ImageFactory"/> class
        /// that this method extends.
        /// </param>
        /// <returns>
        /// The current instance of the <see cref="T:ImageProcessor.ImageFactory"/> class.
        /// </returns>
        public static ImageFactory AutoProcess(this ImageFactory factory)
        {
            if (factory.ShouldProcess)
            {
                // Get a list of all graphics processors that have parsed and matched the querystring.
                List<IGraphicsProcessor> list =
                    ImageProcessorConfig.Instance.GraphicsProcessors
                    .Where(x => x.MatchRegexIndex(factory.QueryString) != int.MaxValue)
                    .OrderBy(y => y.SortOrder)
                    .ToList();

                // Loop through and process the image.
                foreach (IGraphicsProcessor graphicsProcessor in list)
                {
                    try
                    {
                        // TODO: This is going to be a bottleneck for speed. Find a faster way.
                        IGraphicsProcessor processor =
                            (IGraphicsProcessor)Activator.CreateInstance(graphicsProcessor.GetType());
                        // Get the dynamic parameter.
                        processor.MatchRegexIndex(factory.QueryString);
                        // Process.
                        factory.Image = processor.ProcessImage(factory);

                        //// TODO: This is going to be a bottleneck for speed. Find a faster way.
                        //IGraphicsProcessor processor =
                        //    (IGraphicsProcessor)Activator.CreateInstance(graphicsProcessor.GetType());
                        //// Get the dynamic parameter.
                        //processor.MatchRegexIndex(factory.QueryString);
                        //// Process.
                        //factory.Image = processor.ProcessImage(factory);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return factory;
        }


    }
}
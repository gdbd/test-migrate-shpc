﻿using SharepointCommon.Attributes;

// ReSharper disable once CheckNamespace
namespace SharepointCommon
{// <summary>
    /// Base entity for present files in document library (items of content type 'Document')
    /// Used as root of inheritance for all custom entities and content types for doc libraries.
    /// </summary>
    [ContentType("0x0101")]
    public class Document : Item
    {
        /// <summary>
        /// Gets or sets the name of file in document library.
        /// </summary>
        /// <value>
        /// The name of file.
        /// </value>
        [NotMapped]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the content of file in document library.
        /// </summary>
        /// <value>
        /// The content of file.
        /// </value>
        [NotMapped]
        public virtual byte[] Content { get; set; }

        /// <summary>
        /// Gets the size of file in document library.
        /// </summary>
        [NotMapped]
        public virtual long Size { get; protected internal set; }

        /// <summary>
        /// Gets the url, represents image used as icon for file in document library.
        /// </summary>
        [NotMapped]
        public virtual string Icon { get; protected internal set; }

        /// <summary>
        /// Gets the URL of file in document library.
        /// </summary>
        [NotMapped]
        public virtual string Url { get; protected internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether file need be rename (ex. should have index "report(1).doc" )
        /// if file with same name already exists in specified folder.
        /// </summary>
        /// <value>
        /// <c>true</c> if [rename if exists]; otherwise, <c>false</c>.
        /// </value>
        [NotMapped]
        public bool RenameIfExists { get; set; }
    }
}
using System;

namespace XpandAddIns.FormatOnSave {
    /// <summary>
    /// Enumeration listing the possible options that the user can choose for which
    /// document languages should be formatted on save.
    /// </summary>
    [Flags]
    public enum DocumentLanguages {
        /// <summary>
        /// Language option flag indicating "no languages selected."
        /// </summary>
        None = 0,

        /// <summary>
        /// Language option flag corresponding to <see cref="DevExpress.DXCore.Constants.Str.Language.CPlusPlus" />.
        /// </summary>
        CPlusPlus = 1,

        /// <summary>
        /// Language option flag corresponding to <see cref="DevExpress.DXCore.Constants.Str.Language.CSharp" />.
        /// </summary>
        CSharp = 2,

        /// <summary>
        /// Language option flag corresponding to <see cref="DevExpress.DXCore.Constants.Str.Language.CSS" />.
        /// </summary>
        Css = 4,

        /// <summary>
        /// Language option flag corresponding to <see cref="DevExpress.DXCore.Constants.Str.Language.HTML" />.
        /// </summary>
        Html = 8,

        /// <summary>
        /// Language option flag corresponding to <see cref="DevExpress.DXCore.Constants.Str.Language.JavaScript" />.
        /// </summary>
        JavaScript = 16,

        /// <summary>
        /// Language option flag corresponding to <see cref="DevExpress.DXCore.Constants.Str.Language.VisualBasic" />.
        /// </summary>
        VisualBasic = 32,

        /// <summary>
        /// Language option flag corresponding to <see cref="DevExpress.DXCore.Constants.Str.Language.XAML" />.
        /// </summary>
        Xaml = 64,

        /// <summary>
        /// Language option flag corresponding to <see cref="DevExpress.DXCore.Constants.Str.Language.XML" />
        /// and <see cref="DevExpress.DXCore.Constants.Str.Language.XMLOnly" />.
        /// </summary>
        Xml = 128
    }
}
namespace eXpand.ExpressApp.Enums {
    /// <summary>
    /// Describes the type of DetailView to be displayed
    /// </summary>
    public enum DetailViewType {
        /// <summary>
        /// DetailView for a new object
        /// </summary>
        New,
        /// <summary>
        /// DetailView to edit an existing object in a root DetailView (e.g. new window)
        /// </summary>
        Root,
        /// <summary>
        /// DetailView to edit an existing object in a nested DetalView (e.g. in MasterDetailMode of a ListView)
        /// </summary>
        Nested,
        NestedAndRoot,
        All
    }
}
namespace Xpand.ExpressApp.MasterDetail.Win {
    struct LevelDefaultViewInfoCreation
    {
        public LevelDefaultViewInfoCreation(int rowHandle, int relationIndex)
            : this()
        {
            RowHandle = rowHandle;
            RelationIndex = relationIndex;
        }

        public int RowHandle { get; set; }
        public int RelationIndex { get; set; }
    }
}
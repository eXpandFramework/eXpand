namespace Xpand.Persistent.Base.ModelDifference{
    public interface IModelCombinePermission {
        ApplicationModelCombineModifier Modifier { get; set; }
        string Difference { get; set; }
    }
}
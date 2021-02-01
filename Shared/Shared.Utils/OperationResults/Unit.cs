namespace Shared.Utils
{
    /// <summary>
    /// a result requires a type, this class simulates a "void" response.
    /// </summary>
    public sealed class Unit
    {
        public static readonly Unit Value = new Unit();
        private Unit() { }
    }
}

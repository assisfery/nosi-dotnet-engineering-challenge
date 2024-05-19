namespace NOS.Engineering.Challenge.Database;

public interface IDatabase<TOut, in TIn>
{

    Task<TOut?> Create(TIn item);
    Task<TOut?> Read(Guid id);
    Task<IEnumerable<TOut?>> ReadAll();

    Task<IEnumerable<TOut?>> SearchContents(String title, List<string> genres);
    Task<TOut?> Update(Guid id, TIn item);
    Task<Guid> Delete(Guid id);

}
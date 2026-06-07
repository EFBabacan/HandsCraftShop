namespace HandCraft.Siparis.Domain.Common
{
    // Tum entity'lerin ortak temel sinifi (generic Id).
    public abstract class BaseEntity<TId>
    {
        public TId Id { get; set; } = default!;
    }

    // Olusturulma bilgisi tasiyan entity'ler icin.
    public abstract class CreationalEntity<TId> : BaseEntity<TId>
    {
        public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
    }
}

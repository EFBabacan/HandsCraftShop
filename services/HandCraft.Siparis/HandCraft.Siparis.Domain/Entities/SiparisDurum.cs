namespace HandCraft.Siparis.Domain.Entities
{
    // Siparis yasam dongusu (admin ilerletir). Timeline sirasi bu degerlere gore.
    public enum SiparisDurum
    {
        Beklemede = 0,
        Onaylandi = 1,
        Hazirlaniyor = 2,
        Kargoda = 3,
        TeslimEdildi = 4,
        Iptal = 5
    }
}

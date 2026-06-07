namespace HandCraft.Ortak.Identity
{
    // Token claim'lerinden kullanici bilgisi okuyan yardimci (Bolum 4 madde 7).
    public interface IIdentityHelperService
    {
        // UserId = ClaimTypes.NameIdentifier (Keycloak 'sub' claim'i)
        string GetUserId();

        // Kullanici adi = preferred_username
        string GetUserName();
    }
}

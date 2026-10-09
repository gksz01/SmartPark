using Microsoft.AspNetCore.Identity;

namespace SmartPark.Infrastructure.Identidade;

/// <summary>Credencial do Identity. Tem o mesmo Id do <c>Usuario</c> de domínio.</summary>
public sealed class ApplicationUser : IdentityUser<Guid>;

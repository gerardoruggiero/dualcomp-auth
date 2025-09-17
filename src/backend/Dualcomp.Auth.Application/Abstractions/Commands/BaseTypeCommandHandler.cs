using Dualcomp.Auth.Application.Abstractions.Messaging;
using DualComp.Infraestructure.Data.Persistence;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.Application.Abstractions.Commands
{
    /// <summary>
    /// Handler base genérico para crear tipos de entidades
    /// </summary>
    /// <typeparam name="TEntity">Tipo de entidad</typeparam>
    /// <typeparam name="TRepository">Tipo de repositorio</typeparam>
    public abstract class BaseCreateTypeCommandHandler<TEntity, TRepository> 
        : ICommandHandler<BaseCreateTypeCommand<TEntity>, BaseTypeResult<TEntity>>
        where TEntity : BaseTypeEntity
        where TRepository : IRepository<TEntity>
    {
        protected readonly TRepository _repository;
        protected readonly IUnitOfWork _unitOfWork;

        protected BaseCreateTypeCommandHandler(TRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public virtual async Task<BaseTypeResult<TEntity>> Handle(BaseCreateTypeCommand<TEntity> command, CancellationToken cancellationToken)
        {
            // Validaciones comunes
            if (string.IsNullOrWhiteSpace(command.Name))
                throw new ArgumentException("Name is required", nameof(command.Name));

            if (command.Name.Length > 50)
                throw new ArgumentException("Name cannot exceed 50 characters", nameof(command.Name));

            if (!string.IsNullOrWhiteSpace(command.Description) && command.Description.Length > 200)
                throw new ArgumentException("Description cannot exceed 200 characters", nameof(command.Description));

            // Validar unicidad del nombre
            await ValidateNameUniqueness(command.Name, cancellationToken);

            // Crear entidad usando factory method
            var entity = CreateEntity(command.Name, command.Description);

            // Guardar entidad
            await _repository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new BaseTypeResult<TEntity>(entity.Id, entity.Name, entity.Description, entity.IsActive);
        }

        /// <summary>
        /// Método abstracto para crear la entidad específica
        /// </summary>
        protected abstract TEntity CreateEntity(string name, string? description);

        /// <summary>
        /// Método abstracto para validar unicidad del nombre
        /// </summary>
        protected abstract Task ValidateNameUniqueness(string name, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Handler base genérico para actualizar tipos de entidades
    /// </summary>
    /// <typeparam name="TEntity">Tipo de entidad</typeparam>
    /// <typeparam name="TRepository">Tipo de repositorio</typeparam>
    public abstract class BaseUpdateTypeCommandHandler<TEntity, TRepository> 
        : ICommandHandler<BaseUpdateTypeCommand<TEntity>, BaseTypeResult<TEntity>>
        where TEntity : BaseTypeEntity
        where TRepository : IRepository<TEntity>
    {
        protected readonly TRepository _repository;
        protected readonly IUnitOfWork _unitOfWork;

        protected BaseUpdateTypeCommandHandler(TRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public virtual async Task<BaseTypeResult<TEntity>> Handle(BaseUpdateTypeCommand<TEntity> command, CancellationToken cancellationToken)
        {
            // Validaciones comunes
            if (string.IsNullOrWhiteSpace(command.Name))
                throw new ArgumentException("Name is required", nameof(command.Name));

            if (command.Name.Length > 50)
                throw new ArgumentException("Name cannot exceed 50 characters", nameof(command.Name));

            if (!string.IsNullOrWhiteSpace(command.Description) && command.Description.Length > 200)
                throw new ArgumentException("Description cannot exceed 200 characters", nameof(command.Description));

            // Buscar entidad existente
            var entity = await _repository.GetByIdAsync(command.Id, cancellationToken);
            if (entity == null)
                throw new InvalidOperationException($"Entity with ID {command.Id} not found");

            // Validar unicidad del nombre (excluyendo la entidad actual)
            await ValidateNameUniqueness(command.Name, command.Id, cancellationToken);

            // Actualizar entidad
            UpdateEntity(entity, command.Name, command.Description);

            // Guardar cambios
            await _repository.UpdateAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new BaseTypeResult<TEntity>(entity.Id, entity.Name, entity.Description, entity.IsActive);
        }

        /// <summary>
        /// Método abstracto para actualizar la entidad específica
        /// </summary>
        protected abstract void UpdateEntity(TEntity entity, string name, string? description);

        /// <summary>
        /// Método abstracto para validar unicidad del nombre (excluyendo entidad actual)
        /// </summary>
        protected abstract Task ValidateNameUniqueness(string name, Guid excludeId, CancellationToken cancellationToken);
    }
}

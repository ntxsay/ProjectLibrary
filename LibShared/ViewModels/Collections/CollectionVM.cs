using Newtonsoft.Json;
using System;
namespace LibShared.ViewModels.Collections
{
	public class CollectionVM : GenericVM
	{
        [JsonIgnore]
        [Obsolete($"La propriété {nameof(Guid)} n'est pas supportée dans {nameof(CollectionVM)} et lèvera une {nameof(NotSupportedException)} s'il y a usage.", true)]
        ///<summary>La propriété <see cref="Guid"/> n'est pas supportée dans<see cref="CollectionVM"/> et lèvera une <see cref="NotSupportedException"/> s'il y a usage."</summary>
        public override Guid Guid
        {
            get => throw new NotSupportedException($"La propriété {nameof(Guid)} n'est pas supportée dans {nameof(CollectionVM)}.");
            protected set => throw new NotSupportedException($"La propriété {nameof(Guid)} n'est pas supportée dans {nameof(CollectionVM)}.");
        }

        [JsonIgnore]
        [Obsolete($"La propriété {nameof(DateAjout)} n'est pas supportée dans {nameof(CollectionVM)} et lèvera une {nameof(NotSupportedException)} s'il y a usage.", true)]
        ///<summary>La propriété <see cref="DateAjout"/> n'est pas supportée dans<see cref="CollectionVM"/> et lèvera une <see cref="NotSupportedException"/> s'il y a usage."</summary>
        public override DateTime DateAjout
        {
            get => throw new NotSupportedException($"La propriété {nameof(DateAjout)} n'est pas supportée dans {nameof(CollectionVM)}.");
            protected set => throw new NotSupportedException($"La propriété {nameof(DateAjout)} n'est pas supportée dans {nameof(CollectionVM)}.");
        }

        [JsonIgnore]
        [Obsolete($"La propriété {nameof(DateEdition)} n'est pas supportée dans {nameof(CollectionVM)} et lèvera une {nameof(NotSupportedException)} s'il y a usage.", true)]
        ///<summary>La propriété <see cref="DateEdition"/> n'est pas supportée dans<see cref="CollectionVM"/> et lèvera une <see cref="NotSupportedException"/> s'il y a usage."</summary>
        public override DateTime? DateEdition
        {
            get => throw new NotSupportedException($"La propriété {nameof(DateEdition)} n'est pas supportée dans {nameof(CollectionVM)}.");
            protected set => throw new NotSupportedException($"La propriété {nameof(DateEdition)} n'est pas supportée dans {nameof(CollectionVM)}.");
        }

        public long IdLibrary { get; set; } = -1;
	}
}


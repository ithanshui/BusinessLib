/*==================================
             ########
            ##########

             ########
            ##########
          ##############
         #######  #######
        ######      ######
        #####        #####
        ####          ####
        ####   ####   ####
        #####  ####  #####
         ################
          ##############
==================================*/

using System.Linq;

namespace Business.Entity
{
    public abstract class EntityBase
    {
        public static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Member> MetaData;

        static EntityBase()
        {
            MetaData = new System.Collections.Concurrent.ConcurrentDictionary<string, Member>();
            var ass = System.AppDomain.CurrentDomain.GetAssemblies();

            foreach (var item in ass)
            {
                var types = item.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(EntityBase)))
                    {
                        MetaData.TryAdd(type.FullName, GetMetaData(type));
                    }
                }
            }
        }

        public static System.Collections.Generic.List<object> GetArray<Entity>(Entity entity, params string[] members)
            where Entity : EntityBase
        {
            if (null == entity) { return null; }

            var name = entity.GetType().FullName;

            Member meta;

            if (MetaData.TryGetValue(name, out meta))
            {
                var list = new System.Collections.Generic.List<object>();

                var member = new System.Collections.Generic.List<System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>>();

                if (0 < members.Length)
                {
                    foreach (var item in members)
                    {
                        if (meta.MemberAccessor.ContainsKey(item)) { member.Add(meta.MemberAccessor[item]); }
                    }
                }
                else
                {
                    member.AddRange(meta.MemberAccessor.Values);
                }

                if (0 < member.Count)
                {
                    foreach (var item in member)
                    {
                        list.Add(item.Item2(entity));
                    }

                    return list;
                }
            }

            return null;
        }

        public static System.Collections.Generic.List<System.Collections.Generic.List<object>> GetArrayList<Entity>(System.Collections.Generic.IEnumerable<Entity> entitys, params string[] members)
            where Entity : EntityBase
        {
            if (null == entitys || 0 == entitys.Count()) { return null; }

            var name = entitys.ElementAt(0).GetType().FullName;

            Member meta;

            if (MetaData.TryGetValue(name, out meta))
            {
                var list = new System.Collections.Generic.List<System.Collections.Generic.List<object>>();

                var member = new System.Collections.Generic.List<System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>>();

                if (0 < members.Length)
                {
                    foreach (var item in members)
                    {
                        if (meta.MemberAccessor.ContainsKey(item)) { member.Add(meta.MemberAccessor[item]); }
                    }
                }
                else
                {
                    member.AddRange(meta.MemberAccessor.Values);
                }

                //var member = meta.MemberAccessor.Where(c => 0 < members.Length ? members.Contains(c.Key) : true);

                if (0 < member.Count)
                {
                    foreach (var item in entitys)
                    {
                        var list1 = new System.Collections.Generic.List<object>();

                        foreach (var item1 in member)
                        {
                            list1.Add(item1.Item2(item));
                        }
                        list.Add(list1);
                    }

                    return list;
                }
            }

            return null;
        }

        static Member GetMetaData(System.Type type)
        {
            var member = new Member(new System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>>());

            var fields = type.GetFields();
            foreach (var field in fields)
            {
                member.MemberAccessor.Add(field.Name, System.Tuple.Create(field.FieldType, cmstar.RapidReflection.Emit.FieldAccessorGenerator.CreateGetter(field), cmstar.RapidReflection.Emit.FieldAccessorGenerator.CreateSetter(field)));
            }

            var propertys = type.GetProperties();
            foreach (var property in propertys)
            {
                member.MemberAccessor.Add(property.Name, System.Tuple.Create(property.PropertyType, cmstar.RapidReflection.Emit.PropertyAccessorGenerator.CreateGetter(property), cmstar.RapidReflection.Emit.PropertyAccessorGenerator.CreateSetter(property)));
            }

            return member;
        }

        public struct Member
        {
            public Member(System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>> memberAccessor)
            {
                this.memberAccessor = memberAccessor;
            }

            readonly System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>> memberAccessor;
            public System.Collections.Generic.Dictionary<string, System.Tuple<System.Type, System.Func<object, object>, System.Action<object, object>>> MemberAccessor { get { return memberAccessor; } }
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}

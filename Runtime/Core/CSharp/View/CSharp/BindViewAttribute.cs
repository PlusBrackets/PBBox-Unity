using System;
using System.Threading.Tasks;

namespace PBBox.View
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public abstract class BindViewAttributeBase : Attribute
    {
        public string Id { get; private set; }
        /// <summary>
        /// 是否创建表达式创建器
        /// <para>如果为true，则在创建时会使用表达式树来创建，适用于频繁创建该Controller的场景(>20)</para>
        /// </summary>
        public bool IsCreateExpressionFactory { get; private set; } = false;

        /// <summary>
        /// 绑定视图特性基类
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isCreateExpressionCreator">如果为true，则在创建Controller时会使用表达式树来创建，适用于频繁创建该Controller的场景(>20)</param>
        public BindViewAttributeBase(string id)
        {
            this.Id = id;
        }

        public abstract IViewFactory GetViewFactory();
    }
}
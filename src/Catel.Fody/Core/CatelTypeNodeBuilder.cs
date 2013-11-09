﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatelTypeNodeBuilder.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Fody
{
    using System.Collections.Generic;
    using System.Linq;
    using Mono.Cecil;

    public class CatelTypeNodeBuilder
    {
        #region Fields
        private readonly List<TypeDefinition> _allClasses;
        private readonly ModuleWeaver _moduleWeaver;
        private ModuleDefinition _moduleDefinition;

        public List<CatelType> CatelTypes { get; private set; }

        #endregion

        #region Constructors
        public CatelTypeNodeBuilder(ModuleWeaver moduleWeaver, List<TypeDefinition> allTypes)
        {
            _moduleWeaver = moduleWeaver;

            _allClasses = allTypes.Where(x => x.IsClass).ToList();
        }
        #endregion

        #region Methods
        public void Execute()
        {
            _moduleDefinition = _moduleWeaver.ModuleDefinition;

            CatelTypes = new List<CatelType>();
            foreach (var typeDefinition in _allClasses)
            {
                AddCatelTypeIfRequired(typeDefinition);
            }
        }

        private void AddCatelTypeIfRequired(TypeDefinition typeDefinition)
        {
            if (typeDefinition == null)
            {
                return;
            }

            if (typeDefinition.BaseType == null)
            {
                return;
            }

            if (typeDefinition.IsDecoratedWithAttribute("Catel.Fody.NoWeavingAttribute"))
            {
                typeDefinition.RemoveAttribute("Catel.Fody.NoWeavingAttribute");
                return;
            }

            if (!typeDefinition.ImplementsCatelModel())
            {
                return;
            }

            var typeNode = new CatelType(typeDefinition);
            if (CatelTypes.Contains(typeNode))
            {
                return;
            }

            CatelTypes.Add(typeNode);
        }
        #endregion
    }
}
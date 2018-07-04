// <copyright file="CipCreatureConfiguration.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenTibia.Data.Models;

    public class CipCreatureConfiguration : IEntityTypeConfiguration<CipCreature>
    {
        public void Configure(EntityTypeBuilder<CipCreature> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(t => t.Race)
                .IsRequired();

            builder.Property(t => t.Plural)
                .IsRequired();

            builder.Property(t => t.Description)
                .IsRequired();
        }
    }
}

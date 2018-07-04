// <copyright file="HouseTransferConfiguration.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OpenTibia.Data.Models;

    public class HouseTransferConfiguration : IEntityTypeConfiguration<HouseTransfer>
    {
        public void Configure(EntityTypeBuilder<HouseTransfer> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(t => t.HouseId)
                .IsRequired();
        }
    }
}

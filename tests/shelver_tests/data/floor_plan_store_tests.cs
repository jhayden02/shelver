/***********************************************************************************************
*
*   floor_plan_store_tests.cs
*
*   Copyright (c) 2026 Josh Hayden (@jhayden02)
*
*   shelver is free software: you can redistribute it and/or modify
*   it under the terms of the GNU General Public License v3.0 as published
*   by the Free Software Foundation.
*
*   shelver is distributed in the hope that it will be useful,
*   but WITHOUT ANY WARRANTY; without even the implied warranty of
*   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*   GNU General Public License for more details.
*
*   You should have received a copy of the GNU General Public License
*   along with this program.  If not, see <https://www.gnu.org/licenses/>.
*
***********************************************************************************************/

using shelver.data;
using shelver.models;

namespace shelver.tests.data;

public class floor_plan_store_tests
{
    [Fact]
    public void find_floor_plan_returns_plan_for_known_id()
    {
        floor_plan_store plan_store = new floor_plan_store();

        floor_plan? plan = plan_store.find_floor_plan(2);

        Assert.NotNull(plan);
        Assert.Equal("Office", plan.name);
    }

    [Fact]
    public void find_floor_plan_returns_null_for_unknown_id()
    {
        floor_plan_store plan_store = new floor_plan_store();

        floor_plan? plan = plan_store.find_floor_plan(999);

        Assert.Null(plan);
    }
}

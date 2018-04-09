//aggregate(PersonID with min as Minimum)
//groupBy((PhoneNumber), aggregate(PersonID with sum as Total), aggregate(PersonID with min as Minimum))
//groupBy((PhoneNumber, FaxNumber), aggregate(PersonID with sum as Total), aggregate(PersonID with min as Minimum))
//groupBy((FullName), aggregate(PersonID with sum as Total))

QUnit.cases
    .combinatorial([
        { groupby: "(PhoneNumber)" },
        { groupby: null },
        { groupby: "(PhoneNumber, FaxNumber)" }
    ])
    .sequential([
        { orderby: "PhoneNumber" },
        { orderby: null },
        { orderby: "PhoneNumber" }
        
    ])
    .combinatorial([
        { agg: "PersonID with sum as Total" },
        { agg: "2*PersonID with min as Minimum" },
        { agg: "len(FullName)*PersonID with max as Maximum" }
    ])
    .test("agg test", function(params, assert) {
    var done = assert.async();
        var data = null;
        $.ajax("/odata?" + (params.orderby ? "$orderby=" + params.orderby:"") +
            "&$apply=(" + (params.groupby ? "groupby(" + params.groupby + ")," : "") +
            "aggregate(" + params.agg+ "))"
            , { dataType: "json" })
        .done(result => {
            assert.ok(result.value !== null, "Response is retrieved");
        });
        setTimeout(done,1000);
    });
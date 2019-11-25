using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAllDataProvider : IDataProvider {

    public override object GetData()
    {
        return annotations;
    }
}

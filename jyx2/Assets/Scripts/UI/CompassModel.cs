using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassModel
{
    private bool _isVisible;

    public bool isVisible
    {
        get { return _isVisible; }
        set
        {
            var temp = _isVisible;
            _isVisible = value;
            if (_isVisible != temp)
                ValueChanged();
        }
    }

    public delegate void ValueChange(object sender, EventArgs e);

    public event ValueChange OnValueChanged;

    private void ValueChanged()
    {
        if (OnValueChanged != null)
        {
            OnValueChanged(this, null);
        }
    }
}

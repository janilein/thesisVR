using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relation
{

    public string form { get; set; }
    public int Amount { get; set; }

    public Relation(string form, int amount)
    {
        this.form = form;
        this.Amount = amount;
        RemoveAmountFromForm();
    }

    public void RemoveAmountFromForm()
    {
        if(form == null || Amount == 0)
        {
            Debug.Log("No form or amount given");
            throw new Exception();
        }

        string amountString = Amount.ToString();
        form = form.Replace(amountString, "");
        form = form.Trim();

        //Debug.Log("Trimmed new form: " + form);
    }

}

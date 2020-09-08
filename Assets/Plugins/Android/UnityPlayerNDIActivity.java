package com.DefaultCompany.KlaKNDI;

import com.unity3d.player.UnityPlayerActivity;
import android.os.Bundle;
import android.content.Context;
import android.net.nsd.NsdManager; 

public class UnityPlayerNDIActivity extends UnityPlayerActivity {

private NsdManager mNsdManager;

  @Override protected void onCreate(Bundle savedInstanceState) {

    super.onCreate(savedInstanceState);

    mNsdManager = (NsdManager)getApplicationContext().getSystemService(Context.NSD_SERVICE);
  }

}
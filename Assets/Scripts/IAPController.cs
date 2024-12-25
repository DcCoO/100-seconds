using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPController : SingletonMonoBehaviour<IAPController>, IDetailedStoreListener
{

    [Serializable]
    public class IAPItem
    {
        public string Name;
        public string ID;
        public string Description;
        public float Price;
        public ProductType Type;
    }
    
    [SerializeField] private IAPItem _noAds;
    
    private IStoreController _storeController;
    
    protected override void Awake()
    {
        base.Awake();
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(_noAds.ID, _noAds.Type);
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _storeController = controller;
    }
    
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        throw new NotImplementedException();
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;
        var id = product.definition.id;
        
        if (id == _noAds.ID)
        {
            Menu.SetNoAds(true);
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        throw new NotImplementedException();
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        throw new NotImplementedException();
    }
    
    public void BuyNoAds()
    {
        _storeController.InitiatePurchase(_noAds.ID);
    }
}

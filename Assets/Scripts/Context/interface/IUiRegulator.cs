using System;

namespace Snake {

    /// <summary>
    /// ui监管
    /// </summary>
    public interface IUiRegulator : IRegulator {

        void GetUi<T>(string uiName, Action<T> callBack);

        void Open(string uiName);

        void Back(string uiName);

        void Close(string uiName);

        bool Remove(string name);

        void Clear();
    }
}
using Microsoft.JSInterop;
using System.Text.Json;

namespace UVGramWeb.Services;

public interface ILocalStorageService
{
    Task<T> GetItem<T>(string key);
    Task SetItem<T>(string key, T value);
    Task RemoveItem(string key);
}

public class LocalStorageService : ILocalStorageService
{
    private IJSRuntime jsRuntime;
    public LocalStorageService(IJSRuntime jSRuntime)
    {
        this.jsRuntime = jSRuntime;
    }

    public async Task<T> GetItem<T>(string key)
    {
        var json = await jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
        if(json == null ) return default;
        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetItem<T>(string key, T value)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
    }

    public async Task RemoveItem(string key)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }
}
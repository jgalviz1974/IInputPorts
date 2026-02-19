# Gasolutions Core Interfaces Ports - Test Suite

Comprehensive unit and integration tests for the Port interfaces implementing the Clean Architecture pattern.

## Overview

Esta suite de pruebas proporciona cobertura completa para la librería `Gasolutions.Core.Interfaces.Ports`, que define las interfaces para puertos de entrada y salida en implementaciones de Clean Architecture.

## Suite de Pruebas

### 1. IInputPortTests
Pruebas para la interfaz `IInputPort` no genérica.

**Casos de Prueba (8 tests):**
- ? Ejecución sin parámetros
- ? Múltiples ejecuciones secuenciales
- ? Tracking de tiempo de ejecución
- ? Actualización secuencial de tiempo
- ? ValueTask completado
- ? Seguridad en ejecución concurrente
- ? Reusabilidad del puerto

### 2. IInputPortGenericTests
Pruebas para la interfaz genérica `IInputPort<T>`.

**Casos de Prueba (8 tests):**
- ? Ejecución con entidad válida
- ? Procesamiento múltiple de entidades
- ? Manejo de entidad nula
- ? Procesamiento de objetos complejos
- ? ValueTask completado
- ? Ejecución repetida
- ? Manejo de colecciones

### 3. IOutputPortTests
Pruebas para la interfaz `IOutputPort` no genérica.

**Casos de Prueba (7 tests):**
- ? Manejo de resultado
- ? Conteo de múltiples llamadas
- ? Almacenamiento de resultados
- ? Tracking del último resultado
- ? Consistencia de resultados
- ? Ejecución secuencial
- ? Verificación de retorno

### 4. IOutputPortGenericTests
Pruebas para la interfaz genérica `IOutputPort<T>`.

**Casos de Prueba (8 tests):**
- ? Manejo de resultado exitoso
- ? Manejo de resultado con error
- ? Procesamiento de múltiples resultados
- ? Tracking del último resultado
- ? Resultados con objetos complejos
- ? Datos nulos en resultados
- ? Resultados mixtos (éxito/error)
- ? Resultados con colecciones

### 5. IInputPortTwoParametersTests
Pruebas para la interfaz `IInputPort<T1, T2>` con dos parámetros.

**Casos de Prueba (10 tests):**
- ? Ejecución con dos entidades
- ? Procesamiento de múltiples pares
- ? Null en primer parámetro
- ? Null en segundo parámetro
- ? Ambos parámetros nulos
- ? Objetos complejos como parámetros
- ? Colecciones como parámetros
- ? Diferentes tipos de datos
- ? ValueTask completado

### 6. PortIntegrationTests
Pruebas de integración end-to-end para flujos de trabajo completos.

**Casos de Prueba (8+ tests):**
- ? Flujo de trabajo completo
- ? Múltiples casos de uso independientes
- ? Estado de ejecución secuencial
- ? Seguridad en ejecución concurrente
- ? Propagación de errores
- ? Encadenamiento de puertos de entrada
- ? Independencia de puertos de salida
- ? Reusabilidad y consistencia

---

## ?? Estadísticas

| Métrica | Valor |
|---------|-------|
| **Total de Clases de Prueba** | 6 |
| **Total de Test Cases** | 50+ |
| **Métodos [Fact]** | 45+ |
| **Métodos [Theory]** | 5+ |
| **Líneas de Código** | ~900+ |
| **Cobertura Esperada** | 95%+ |

---

## ?? Ejecución de Pruebas

```bash
# Ejecutar todas las pruebas
dotnet test

# Ejecutar clase específica
dotnet test --filter "ClassName=IInputPortTests"

# Con cobertura
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover

# Salida detallada
dotnet test -v detailed
```

---

## ?? Características de las Pruebas

| Característica | Estado |
|---|---|
| **Documentación XML** | ? Cada test incluye summary |
| **Patrón AAA** | ? Arrange-Act-Assert |
| **Aislamiento** | ? Sin dependencias externas |
| **Mocks incluidos** | ? En cada clase de prueba |
| **Edge cases** | ? Null, vacío, concurrente |
| **Performance** | ? Concurrent execution |
| **Integración** | ? Flujos completos |
| **Thread-safety** | ? Validación de seguridad |

---

## ?? Dependencias

```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
<PackageReference Include="xunit" Version="2.9.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
<PackageReference Include="coverlet.collector" Version="6.0.2" />
<PackageReference Include="Gasolutions.Core.Patterns.Result" Version="1.0.7" />
```

---

## ?? Patrones de Prueba Comunes

### Patrón AAA
```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange - Preparar datos
    var port = new MockInputPort<string>();
    const string testEntity = "test";

    // Act - Ejecutar
    await port.Execute(testEntity);

    // Assert - Verificar
    Assert.NotNull(port.ReceivedEntity);
}
```

### Mock Implementations
```csharp
private class MockInputPort<T> : IInputPort<T>
{
    public List<T?> ReceivedEntities { get; } = new();
    
    public ValueTask Execute(T? entity)
    {
        ReceivedEntities.Add(entity);
        return ValueTask.CompletedTask;
    }
}
```

---

## ?? Ejemplos de Uso

```csharp
// Entrada simple
var port = new MockInputPort<string>();
await port.Execute("data");
Assert.Single(port.ReceivedEntities);

// Resultado
var outputPort = new MockOutputPort<string>();
var result = Result<string>.Success("data");
outputPort.Handle(result);
Assert.True(outputPort.LastResult.IsSuccess);

// Dos parámetros
var twoParamPort = new MockInputPortTwoParameters<string, int>();
await twoParamPort.Execute("data", 42);
Assert.Equal(("data", 42), twoParamPort.ReceivedEntities[0]);
```

---

## ? Características Testeadas

? Ejecución de puerto de entrada con y sin parámetros
? Manejo de resultados en puerto de salida
? Soporte de parámetros genéricos (1 a 2 parámetros)
? Manejo de valores nulos y vacíos
? Procesamiento de objetos complejos
? Manejo de colecciones
? Ejecución concurrente thread-safe
? Estados de resultado (éxito/error)
? Flujos de trabajo secuenciales y paralelos
? Reusabilidad de puertos

---

## ?? Cobertura de Pruebas

**Objetivos de Cobertura:**
- Cobertura General: 95%+
- Cobertura de Líneas: 95%+
- Cobertura de Ramas: 90%+

---

**Última Actualización:** 2024
**Versión:** 1.0.7

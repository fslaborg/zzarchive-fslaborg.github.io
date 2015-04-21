namespace global
module fsi =
  let AddPrinter(a) = ()

[<AutoOpen>]
module RecoverBangOperator =
  let (!) (a:ref<_>) = a.Value 

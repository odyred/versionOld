﻿using ISAAR.MSolve.Analyzers;
using ISAAR.MSolve.Logging;
using ISAAR.MSolve.Matrices;
using ISAAR.MSolve.PreProcessor;
using ISAAR.MSolve.Problems;
using ISAAR.MSolve.Solvers.Skyline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.SamplesConsole
{
    class Program
    {
        private static void SolveBuildingInNoSoilSmall()
        {
            VectorExtensions.AssignTotalAffinityCount();
            Model model = new Model();
            model.SubdomainsDictionary.Add(1, new Subdomain() { ID = 1 });
            BeamBuildingBuilder.MakeBeamBuilding(model, 20, 20, 20, 5, 4, model.NodesDictionary.Count + 1,
                model.ElementsDictionary.Count + 1, 1, 4, false, false);
            model.Loads.Add(new Load() { Amount = -100, Node = model.Nodes[21], DOF = DOFType.X });
            model.ConnectDataStructures();

            SolverSkyline solver = new SolverSkyline(model);
            ProblemStructural provider = new ProblemStructural(model, solver.SubdomainsDictionary);
            LinearAnalyzer analyzer = new LinearAnalyzer(solver, solver.SubdomainsDictionary);
            StaticAnalyzer parentAnalyzer = new StaticAnalyzer(provider, analyzer, solver.SubdomainsDictionary);

            analyzer.LogFactories[1] = new LinearAnalyzerLogFactory(new int[] { 420 });

            parentAnalyzer.BuildMatrices();
            parentAnalyzer.Initialize();
            parentAnalyzer.Solve();
        }


        private static void SolveBuildingInNoSoilSmallVRFStochastic()
        {
            VectorExtensions.AssignTotalAffinityCount();
            Model model = new Model();
            model.SubdomainsDictionary.Add(1, new Subdomain() { ID = 1 });
            BeamBuildingBuilder.MakeBeamBuilding(model, 20, 20, 20, 5, 4, model.NodesDictionary.Count + 1,
                model.ElementsDictionary.Count + 1, 1, 4, false, false);
            model.Loads.Add(new Load() { Amount = -100, Node = model.Nodes[21], DOF = DOFType.X });
            model.ConnectDataStructures();

            PowerSpectrumTargetEvaluatorCoefficientsProvider stochasticProvider = new PowerSpectrumTargetEvaluatorCoefficientsProvider(10, 0.1, 1.2, 20, 200, DOFType.X,
                0.1, 200, 1e-10);


            SolverSkyline solver = new SolverSkyline(model);
            ProblemStructural provider = new ProblemStructural(model, solver.SubdomainsDictionary);
            LinearAnalyzer analyzer = new LinearAnalyzer(solver, solver.SubdomainsDictionary);
            StaticAnalyzer childAnalyzer = new StaticAnalyzer(provider, analyzer, solver.SubdomainsDictionary);
            VRFMonteCarloAnalyzerWithStochasticMaterial stochasticAnalyzer = new VRFMonteCarloAnalyzerWithStochasticMaterial(model, provider, childAnalyzer, solver.SubdomainsDictionary,
                stochasticProvider, stochasticProvider, 1, 20, "monteCarlo");


            analyzer.LogFactories[1] = new LinearAnalyzerLogFactory(new int[] { 420 });

            //stochasticAnalyzer.BuildMatrices();
            //childAnalyzer.Initialize();
            stochasticAnalyzer.Solve();
        }

        private static void SolveBuildingInNoSoilSmallDynamic()
        {
            VectorExtensions.AssignTotalAffinityCount();
            Model model = new Model();
            model.SubdomainsDictionary.Add(1, new Subdomain() { ID = 1 });
            BeamBuildingBuilder.MakeBeamBuilding(model, 20, 20, 20, 5, 4, model.NodesDictionary.Count + 1,
                model.ElementsDictionary.Count + 1, 1, 4, false, false);
            model.ConnectDataStructures();

            SolverSkyline solver = new SolverSkyline(model);
            ProblemStructural provider = new ProblemStructural(model, solver.SubdomainsDictionary);
            LinearAnalyzer analyzer = new LinearAnalyzer(solver, solver.SubdomainsDictionary);
            NewmarkDynamicAnalyzer parentAnalyzer = new NewmarkDynamicAnalyzer(provider, analyzer, solver.SubdomainsDictionary, 0.5, 0.25, 0.01, 0.1);

            analyzer.LogFactories[1] = new LinearAnalyzerLogFactory(new int[] { 420 });

            parentAnalyzer.BuildMatrices();
            parentAnalyzer.Initialize();
            parentAnalyzer.Solve();
        }


        static void Main(string[] args)
        {
            SolveBuildingInNoSoilSmallVRFStochastic();
        }
    }
}
